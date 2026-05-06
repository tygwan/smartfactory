using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SmartFactory.Piping.Viewer
{
    [RequireComponent(typeof(Camera))]
    public class AuthorCameraController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float runMultiplier = 3f;
        [SerializeField] private float verticalSpeed = 6f;

        [Header("Look")]
        [SerializeField] private float lookSensitivity = 0.15f;
        [SerializeField] private float pitchMin = -89f;
        [SerializeField] private float pitchMax = 89f;

        public event Action<Ray> OnAuthorClick;

        private Camera _camera;
        private InputAction _move;
        private InputAction _vertical;
        private InputAction _look;
        private InputAction _rotate;
        private InputAction _run;
        private InputAction _click;

        private float _yaw;
        private float _pitch;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            var euler = transform.eulerAngles;
            _yaw = euler.y;
            _pitch = NormalizePitch(euler.x);
        }

        private void OnEnable()
        {
            _move = new InputAction("move", InputActionType.Value);
            _move.AddCompositeBinding("2DVector")
                 .With("Up", "<Keyboard>/w")
                 .With("Down", "<Keyboard>/s")
                 .With("Left", "<Keyboard>/a")
                 .With("Right", "<Keyboard>/d");

            _vertical = new InputAction("vertical", InputActionType.Value);
            _vertical.AddCompositeBinding("1DAxis")
                     .With("Negative", "<Keyboard>/q")
                     .With("Positive", "<Keyboard>/e");

            _look = new InputAction("look", InputActionType.Value, "<Mouse>/delta");
            _rotate = new InputAction("rotate", InputActionType.Button, "<Mouse>/rightButton");
            _run = new InputAction("run", InputActionType.Button, "<Keyboard>/leftShift");
            _click = new InputAction("click", InputActionType.Button, "<Mouse>/leftButton");

            _move.Enable();
            _vertical.Enable();
            _look.Enable();
            _rotate.Enable();
            _run.Enable();
            _click.Enable();

            _click.performed += HandleClick;
        }

        private void OnDisable()
        {
            _click.performed -= HandleClick;
            _move?.Dispose();
            _vertical?.Dispose();
            _look?.Dispose();
            _rotate?.Dispose();
            _run?.Dispose();
            _click?.Dispose();
        }

        private void Update()
        {
            if (_rotate.IsPressed())
            {
                var delta = _look.ReadValue<Vector2>();
                _yaw += delta.x * lookSensitivity;
                _pitch -= delta.y * lookSensitivity;
                _pitch = Mathf.Clamp(_pitch, pitchMin, pitchMax);
                transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            }

            var planar = _move.ReadValue<Vector2>();
            var lift = _vertical.ReadValue<float>();

            var local = new Vector3(planar.x, 0f, planar.y);
            var world = transform.TransformDirection(local);
            world.y += lift * (verticalSpeed / Mathf.Max(0.001f, moveSpeed));

            var speed = moveSpeed * (_run.IsPressed() ? runMultiplier : 1f);
            transform.position += world * speed * Time.deltaTime;
        }

        private void HandleClick(InputAction.CallbackContext ctx)
        {
            if (_rotate.IsPressed()) return;
            if (_camera == null) return;

            var pointer = Mouse.current;
            if (pointer == null) return;

            var ray = _camera.ScreenPointToRay(pointer.position.ReadValue());
            OnAuthorClick?.Invoke(ray);
        }

        private static float NormalizePitch(float pitch)
        {
            if (pitch > 180f) pitch -= 360f;
            return pitch;
        }
    }
}
