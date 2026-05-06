#if UNITY_EDITOR
using SmartFactory.Piping.Data;
using UnityEditor;
using UnityEngine;

namespace SmartFactory.Piping.EditorTools
{
    public class PipeSceneAuthoringWindow : EditorWindow
    {
        private const string ActivePrefKey = "SmartFactory.PipeSceneAuthoring.Active";

        [SerializeField] private PipeNetworkAsset network;
        [SerializeField] private float diameter = 0.2f;
        [SerializeField] private string material = "steel";
        [SerializeField] private float fallbackPlaneY = 0f;
        [SerializeField] private float maxRayDistance = 500f;

        private bool _active;
        private bool _hasFirstPoint;
        private Vector3 _firstPoint;

        [MenuItem("SmartFactory/Pipe Scene Authoring")]
        public static void Open()
        {
            var w = GetWindow<PipeSceneAuthoringWindow>("Pipe Authoring");
            w.minSize = new Vector2(280, 180);
        }

        private void OnEnable()
        {
            _active = EditorPrefs.GetBool(ActivePrefKey, false);
            SceneView.duringSceneGui += OnSceneGui;
        }

        private void OnDisable()
        {
            EditorPrefs.SetBool(ActivePrefKey, _active);
            SceneView.duringSceneGui -= OnSceneGui;
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("SmartFactory Pipe Scene Authoring", EditorStyles.boldLabel);
            EditorGUILayout.Space(4);

            network = (PipeNetworkAsset)EditorGUILayout.ObjectField(
                "Pipe Network", network, typeof(PipeNetworkAsset), false);

            diameter = Mathf.Max(0.01f, EditorGUILayout.FloatField("Diameter (m)", diameter));
            material = EditorGUILayout.TextField("Material", material);
            fallbackPlaneY = EditorGUILayout.FloatField("Fallback Plane Y", fallbackPlaneY);

            EditorGUILayout.Space(4);

            using (new EditorGUI.DisabledScope(network == null))
            {
                var newActive = EditorGUILayout.ToggleLeft(
                    "Authoring Active (click in Scene view)",
                    _active && network != null);
                if (newActive != _active)
                {
                    _active = newActive;
                    EditorPrefs.SetBool(ActivePrefKey, _active);
                    if (!_active) ResetFirstPoint();
                    SceneView.RepaintAll();
                }
            }

            EditorGUILayout.Space(4);

            string hint;
            MessageType type;
            if (network == null)
            {
                hint = "Assign a PipeNetworkAsset above to begin.";
                type = MessageType.Warning;
            }
            else if (!_active)
            {
                hint = "Toggle Authoring Active to enable Scene view picking.";
                type = MessageType.Info;
            }
            else if (_hasFirstPoint)
            {
                hint = "Second point: click in Scene view to commit. Esc to cancel.";
                type = MessageType.Info;
            }
            else
            {
                hint = "First point: click anywhere in Scene view (raycast or fallback plane).";
                type = MessageType.Info;
            }
            EditorGUILayout.HelpBox(hint, type);

            using (new EditorGUI.DisabledScope(!_hasFirstPoint))
            {
                if (GUILayout.Button("Cancel In-Progress"))
                {
                    ResetFirstPoint();
                    SceneView.RepaintAll();
                }
            }

            using (new EditorGUI.DisabledScope(network == null || network.Count == 0))
            {
                if (GUILayout.Button("Clear Network"))
                {
                    if (EditorUtility.DisplayDialog(
                        "Clear pipe network",
                        $"Remove all {network.Count} pipes from {network.name}?",
                        "Clear", "Cancel"))
                    {
                        Undo.RegisterCompleteObjectUndo(network, "Clear Pipe Network");
                        network.Clear();
                        EditorUtility.SetDirty(network);
                    }
                }
            }
        }

        private void OnSceneGui(SceneView sceneView)
        {
            if (!_active || network == null) return;

            var ev = Event.current;
            var controlId = GUIUtility.GetControlID(FocusType.Passive);

            if (ev.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(controlId);
            }

            if (ev.type == EventType.MouseDown && ev.button == 0 && !ev.alt && !ev.control && !ev.command)
            {
                var ray = HandleUtility.GUIPointToWorldRay(ev.mousePosition);
                if (TryProjectRay(ray, out var point))
                {
                    if (!_hasFirstPoint)
                    {
                        _firstPoint = point;
                        _hasFirstPoint = true;
                    }
                    else
                    {
                        if (Vector3.Distance(_firstPoint, point) > 1e-3f)
                        {
                            Undo.RegisterCompleteObjectUndo(network, "Add Pipe");
                            network.AddPipe(_firstPoint, point, diameter, material);
                            EditorUtility.SetDirty(network);
                        }
                        _hasFirstPoint = false;
                    }
                    ev.Use();
                    Repaint();
                    sceneView.Repaint();
                }
            }

            if (ev.type == EventType.KeyDown && ev.keyCode == KeyCode.Escape && _hasFirstPoint)
            {
                ResetFirstPoint();
                ev.Use();
                Repaint();
                sceneView.Repaint();
            }

            if (_hasFirstPoint)
            {
                Handles.color = new Color(0.4f, 0.85f, 1f, 1f);
                Handles.SphereHandleCap(0, _firstPoint, Quaternion.identity, diameter * 1.4f, EventType.Repaint);

                var ray = HandleUtility.GUIPointToWorldRay(ev.mousePosition);
                if (TryProjectRay(ray, out var preview))
                {
                    Handles.color = new Color(0.4f, 0.85f, 1f, 0.7f);
                    Handles.DrawDottedLine(_firstPoint, preview, 4f);
                    Handles.color = new Color(0.4f, 0.85f, 1f, 0.5f);
                    Handles.SphereHandleCap(0, preview, Quaternion.identity, diameter * 0.9f, EventType.Repaint);
                }
                sceneView.Repaint();
            }
        }

        private bool TryProjectRay(Ray ray, out Vector3 point)
        {
            if (Physics.Raycast(ray, out var hit, maxRayDistance))
            {
                point = hit.point;
                return true;
            }
            if (Mathf.Abs(ray.direction.y) < 1e-4f)
            {
                point = default;
                return false;
            }
            var t = (fallbackPlaneY - ray.origin.y) / ray.direction.y;
            if (t < 0f)
            {
                point = default;
                return false;
            }
            point = ray.origin + ray.direction * t;
            return true;
        }

        private void ResetFirstPoint()
        {
            _hasFirstPoint = false;
        }
    }
}
#endif
