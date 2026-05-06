using System.Collections.Generic;
using SmartFactory.Piping.Data;
using UnityEngine;

namespace SmartFactory.Piping.Viewer
{
    public class PipeView : MonoBehaviour
    {
        [SerializeField] private PipeNetworkAsset network;
        [SerializeField] private Material pipeMaterial;

        private readonly Dictionary<string, GameObject> _instances = new();

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
        }

        private GameObject CreatePipeGameObject(PipeData pipe)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = $"Pipe_{Shorten(pipe.id)}";
            go.transform.SetParent(transform, worldPositionStays: false);

            var renderer = go.GetComponent<MeshRenderer>();
            if (pipeMaterial != null && renderer != null)
                renderer.sharedMaterial = pipeMaterial;

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
