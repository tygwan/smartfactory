using SmartFactory.Piping.Data;
using SmartFactory.Piping.Viewer;
using UnityEngine;

namespace SmartFactory.Piping.Authoring
{
    public class PipeAuthoringTool : MonoBehaviour
    {
        [SerializeField] private PipeNetworkAsset network;
        [SerializeField] private AuthorCameraController authorCamera;

        [Header("Defaults")]
        [SerializeField] private float defaultDiameter = 0.2f;
        [SerializeField] private string defaultMaterial = "steel";

        [Header("Picking")]
        [SerializeField] private LayerMask raycastMask = ~0;
        [SerializeField] private float maxRayDistance = 200f;
        [SerializeField] private float fallbackPlaneY = 0f;

        [Header("Feedback")]
        [SerializeField] private GameObject firstPointMarker;

        private bool _hasFirstPoint;
        private Vector3 _firstPoint;

        public bool HasFirstPoint => _hasFirstPoint;
        public Vector3 FirstPoint => _firstPoint;

        private void OnEnable()
        {
            if (authorCamera != null)
                authorCamera.OnAuthorClick += HandleClick;
            UpdateMarker();
        }

        private void OnDisable()
        {
            if (authorCamera != null)
                authorCamera.OnAuthorClick -= HandleClick;
        }

        public void CancelInProgress()
        {
            _hasFirstPoint = false;
            UpdateMarker();
        }

        private void HandleClick(Ray ray)
        {
            if (network == null) return;
            if (!TryProjectRay(ray, out var point)) return;

            if (!_hasFirstPoint)
            {
                _firstPoint = point;
                _hasFirstPoint = true;
                UpdateMarker();
                return;
            }

            if (Vector3.Distance(_firstPoint, point) < 1e-3f)
                return;

            network.AddPipe(_firstPoint, point, defaultDiameter, defaultMaterial);
            _hasFirstPoint = false;
            UpdateMarker();
        }

        private bool TryProjectRay(Ray ray, out Vector3 point)
        {
            if (Physics.Raycast(ray, out var hit, maxRayDistance, raycastMask))
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

        private void UpdateMarker()
        {
            if (firstPointMarker == null) return;
            firstPointMarker.SetActive(_hasFirstPoint);
            if (_hasFirstPoint)
                firstPointMarker.transform.position = _firstPoint;
        }
    }
}
