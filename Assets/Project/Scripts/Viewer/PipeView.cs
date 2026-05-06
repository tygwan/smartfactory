using System;
using System.Collections.Generic;
using SmartFactory.Piping.Data;
using SmartFactory.Piping.Validation;
using UnityEngine;

namespace SmartFactory.Piping.Viewer
{
    [ExecuteAlways]
    public class PipeView : MonoBehaviour
    {
        [SerializeField] private PipeNetworkAsset network;
        [SerializeField] private Material pipeMaterial;
        [SerializeField] private Material clashMaterial;
        [SerializeField] private float clashClearance = 0f;

        [Header("Pressure gradient (analytic — M3 will replace with CFD surrogate)")]
        [SerializeField] private Color lowPressureColor = new Color(0.25f, 0.55f, 1f, 1f);
        [SerializeField] private Color highPressureColor = new Color(1f, 0.30f, 0.20f, 1f);

        [Header("Auto-elbow (A2 — sphere fallback)")]
        [SerializeField] private bool autoElbow = true;
        [SerializeField] private float jointBucketSize = 0.05f;
        [SerializeField] private float jointSizeMultiplier = 1.35f;
        [SerializeField] private Material jointMaterial;

        [Header("Layer toggles (B2)")]
        [SerializeField] private bool showClashHighlights = true;

        public bool ShowClashHighlights
        {
            get => showClashHighlights;
            set
            {
                if (showClashHighlights == value) return;
                showClashHighlights = value;
                Refresh();
            }
        }

        public event Action<IReadOnlyList<(int a, int b)>> OnClashesUpdated;

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        private readonly Dictionary<string, GameObject> _instances = new();
        private readonly Dictionary<Vector3Int, GameObject> _joints = new();
        private readonly HashSet<int> _clashingIndices = new();
        private MaterialPropertyBlock _propertyBlock;

        private void OnEnable()
        {
            if (network == null) return;
            network.OnChanged += Refresh;
            Refresh();
        }

        private void OnDisable()
        {
            if (network != null)
                network.OnChanged -= Refresh;
            ClearAll();
        }

        public void Refresh()
        {
            if (network == null) return;

            var stale = new HashSet<string>(_instances.Keys);
            foreach (var pipe in network.Pipes)
            {
                stale.Remove(pipe.id);
                if (!_instances.TryGetValue(pipe.id, out var go) || go == null)
                {
                    go = CreatePipeGameObject(pipe);
                    _instances[pipe.id] = go;
                }
                ApplyTransform(go, pipe);
            }

            foreach (var id in stale)
            {
                if (_instances.TryGetValue(id, out var go))
                {
                    DestroySafe(go);
                    _instances.Remove(id);
                }
            }

            ApplyClashHighlights();
            RebuildJoints();
        }

        private void RebuildJoints()
        {
            var stale = new HashSet<Vector3Int>(_joints.Keys);

            if (autoElbow && network != null && network.Pipes.Count > 0)
            {
                var counts = new Dictionary<Vector3Int, int>();
                var maxDiameter = new Dictionary<Vector3Int, float>();

                foreach (var pipe in network.Pipes)
                {
                    Tally(counts, maxDiameter, BucketKey(pipe.start), pipe.diameter);
                    Tally(counts, maxDiameter, BucketKey(pipe.end), pipe.diameter);
                }

                foreach (var kv in counts)
                {
                    if (kv.Value < 2) continue;
                    var key = kv.Key;
                    stale.Remove(key);

                    if (!_joints.TryGetValue(key, out var go) || go == null)
                    {
                        go = CreateJointSphere();
                        _joints[key] = go;
                    }

                    var diameter = maxDiameter[key] * jointSizeMultiplier;
                    go.transform.position = BucketCenter(key);
                    go.transform.localScale = new Vector3(diameter, diameter, diameter);
                }
            }

            foreach (var key in stale)
            {
                if (_joints.TryGetValue(key, out var go))
                {
                    DestroySafe(go);
                    _joints.Remove(key);
                }
            }
        }

        private GameObject CreateJointSphere()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "PipeJoint";
            go.transform.SetParent(transform, worldPositionStays: false);
            var col = go.GetComponent<Collider>();
            if (col != null) col.enabled = false;
            var renderer = go.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                var mat = jointMaterial != null ? jointMaterial : pipeMaterial;
                if (mat != null) renderer.sharedMaterial = mat;
            }
            return go;
        }

        private static void Tally(
            Dictionary<Vector3Int, int> counts,
            Dictionary<Vector3Int, float> maxDiameter,
            Vector3Int key,
            float diameter)
        {
            counts[key] = counts.TryGetValue(key, out var c) ? c + 1 : 1;
            if (!maxDiameter.TryGetValue(key, out var d) || diameter > d)
                maxDiameter[key] = diameter;
        }

        private Vector3Int BucketKey(Vector3 p)
        {
            var s = Mathf.Max(jointBucketSize, 1e-4f);
            return new Vector3Int(
                Mathf.RoundToInt(p.x / s),
                Mathf.RoundToInt(p.y / s),
                Mathf.RoundToInt(p.z / s));
        }

        private Vector3 BucketCenter(Vector3Int key)
        {
            return new Vector3(key.x, key.y, key.z) * jointBucketSize;
        }

        private void ApplyClashHighlights()
        {
            _clashingIndices.Clear();
            if (network == null) return;

            var clashes = ValidationRules.DetectClashes(network.Pipes, clashClearance);
            foreach (var (i, j) in clashes)
            {
                _clashingIndices.Add(i);
                _clashingIndices.Add(j);
            }

            float maxDp = 0f;
            for (int i = 0; i < network.Pipes.Count; i++)
            {
                var dp = AnalyticPressureDrop(network.Pipes[i]);
                if (dp > maxDp) maxDp = dp;
            }
            if (maxDp < 1e-4f) maxDp = 1f;

            _propertyBlock ??= new MaterialPropertyBlock();

            for (int i = 0; i < network.Pipes.Count; i++)
            {
                var pipe = network.Pipes[i];
                if (!_instances.TryGetValue(pipe.id, out var go) || go == null)
                    continue;

                var renderer = go.GetComponent<MeshRenderer>();
                if (renderer == null) continue;

                var isClash = showClashHighlights && _clashingIndices.Contains(i);
                var target = isClash && clashMaterial != null
                    ? clashMaterial
                    : pipeMaterial;
                if (target != null && renderer.sharedMaterial != target)
                    renderer.sharedMaterial = target;

                if (isClash)
                {
                    renderer.SetPropertyBlock(null);
                }
                else
                {
                    var t = Mathf.Clamp01(AnalyticPressureDrop(pipe) / maxDp);
                    var color = Color.Lerp(lowPressureColor, highPressureColor, t);
                    _propertyBlock.Clear();
                    _propertyBlock.SetColor(BaseColorId, color);
                    renderer.SetPropertyBlock(_propertyBlock);
                }
            }

            OnClashesUpdated?.Invoke(clashes);
        }

        private static float AnalyticPressureDrop(PipeData pipe)
        {
            var d = Mathf.Max(pipe.diameter, 1e-3f);
            return pipe.Length / Mathf.Pow(d, 5f);
        }

        private GameObject CreatePipeGameObject(PipeData pipe)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = $"Pipe_{Shorten(pipe.id)}";
            go.transform.SetParent(transform, worldPositionStays: false);

            var renderer = go.GetComponent<MeshRenderer>();
            if (pipeMaterial != null && renderer != null)
                renderer.sharedMaterial = pipeMaterial;

            var marker = go.AddComponent<PipeViewItem>();
            marker.PipeId = pipe.id;

            return go;
        }

        private static void ApplyTransform(GameObject go, PipeData pipe)
        {
            var dir = pipe.end - pipe.start;
            var len = dir.magnitude;
            if (len < 1e-4f) return;

            go.transform.position = pipe.Center;
            go.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir / len);
            go.transform.localScale = new Vector3(pipe.diameter, len * 0.5f, pipe.diameter);
        }

        private void ClearAll()
        {
            foreach (var kv in _instances)
                DestroySafe(kv.Value);
            _instances.Clear();
            foreach (var kv in _joints)
                DestroySafe(kv.Value);
            _joints.Clear();
        }

        private static void DestroySafe(GameObject go)
        {
            if (go == null) return;
            if (Application.isPlaying) Destroy(go);
            else DestroyImmediate(go);
        }

        private static string Shorten(string id) =>
            string.IsNullOrEmpty(id) ? "?" : id.Substring(0, System.Math.Min(8, id.Length));
    }
}
