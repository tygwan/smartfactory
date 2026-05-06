#if UNITY_EDITOR
using System.IO;
using SmartFactory.Piping.Data;
using UnityEditor;
using UnityEngine;

namespace SmartFactory.Piping.EditorTools
{
    public static class DrawingExportTool
    {
        private const int Resolution = 1024;
        private const string ExportFolder = "Assets/Project/Exports";

        [MenuItem("SmartFactory/Export Top-Down PNG")]
        public static void ExportTopDownPng() => ExportFromTopDown(includeFabBackground: true);

        [MenuItem("SmartFactory/Export Top-Down PNG (pipes only)")]
        public static void ExportTopDownPipesOnly() => ExportFromTopDown(includeFabBackground: false);

        private static void ExportFromTopDown(bool includeFabBackground)
        {
            var network = FindNetwork();
            if (network == null)
            {
                EditorUtility.DisplayDialog(
                    "Export failed",
                    "No PipeNetworkAsset found. Create one under Assets/Project/Data/ first.",
                    "OK");
                return;
            }

            if (network.Count == 0)
            {
                EditorUtility.DisplayDialog(
                    "Export failed",
                    $"{network.name} has no pipes yet.",
                    "OK");
                return;
            }

            var bounds = ComputeBounds(network);
            var maxDim = Mathf.Max(bounds.size.x, bounds.size.z, 1f);
            var orthoSize = maxDim * 0.6f;

            var camGo = new GameObject("DrawingExport_Camera_Temp");
            RenderTexture rt = null;
            Texture2D tex = null;

            try
            {
                var cam = camGo.AddComponent<Camera>();
                cam.orthographic = true;
                cam.orthographicSize = orthoSize;
                cam.transform.position = new Vector3(
                    bounds.center.x,
                    bounds.center.y + bounds.size.y * 0.5f + 50f,
                    bounds.center.z);
                cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = includeFabBackground
                    ? new Color(0.10f, 0.11f, 0.13f, 1f)
                    : Color.white;
                cam.nearClipPlane = 0.1f;
                cam.farClipPlane = 500f;
                cam.cullingMask = includeFabBackground ? ~0 : 0;

                if (!includeFabBackground)
                {
                    var pipeViews = Object.FindObjectsByType<SmartFactory.Piping.Viewer.PipeView>(
                        FindObjectsSortMode.None);
                    foreach (var pv in pipeViews)
                        EnableLayer(cam, pv.gameObject);
                }

                rt = new RenderTexture(Resolution, Resolution, 24);
                cam.targetTexture = rt;

                var prevActive = RenderTexture.active;
                cam.Render();
                RenderTexture.active = rt;

                tex = new Texture2D(Resolution, Resolution, TextureFormat.RGB24, false);
                tex.ReadPixels(new Rect(0, 0, Resolution, Resolution), 0, 0);
                tex.Apply();

                cam.targetTexture = null;
                RenderTexture.active = prevActive;

                var bytes = tex.EncodeToPNG();

                if (!Directory.Exists(ExportFolder))
                    Directory.CreateDirectory(ExportFolder);

                var suffix = includeFabBackground ? "" : "_pipes";
                var path = Path.Combine(ExportFolder, $"M1_top{suffix}.png");
                File.WriteAllBytes(path, bytes);

                AssetDatabase.Refresh();
                Debug.Log($"[Drawing] Top-down PNG exported: {path} " +
                          $"({Resolution}x{Resolution}, ortho={orthoSize:0.00}, " +
                          $"bounds={bounds.size:F2})");
            }
            finally
            {
                if (rt != null)
                {
                    rt.Release();
                    Object.DestroyImmediate(rt);
                }
                if (tex != null) Object.DestroyImmediate(tex);
                Object.DestroyImmediate(camGo);
            }
        }

        private static void EnableLayer(Camera cam, GameObject root)
        {
            var renderers = root.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
                cam.cullingMask |= 1 << r.gameObject.layer;
        }

        private static PipeNetworkAsset FindNetwork()
        {
            var guids = AssetDatabase.FindAssets("t:PipeNetworkAsset");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<PipeNetworkAsset>(path);
                if (asset != null) return asset;
            }
            return null;
        }

        private static Bounds ComputeBounds(PipeNetworkAsset network)
        {
            var first = network.Pipes[0];
            var b = new Bounds(first.start, Vector3.zero);
            b.Encapsulate(first.end);
            for (int i = 1; i < network.Count; i++)
            {
                b.Encapsulate(network.Pipes[i].start);
                b.Encapsulate(network.Pipes[i].end);
            }
            return b;
        }
    }
}
#endif
