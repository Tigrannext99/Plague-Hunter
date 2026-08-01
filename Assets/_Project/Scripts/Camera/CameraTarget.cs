using UnityEngine;
using UnityEngine.InputSystem;

namespace PlagueHunter.Player
{
    public class CameraTargetFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, 0f);

        [Header("Sensitivity")]
        [SerializeField] private float mouseSensitivity = 0.3f;
        [SerializeField] private float gamepadSensitivity = 180f;

        [Header("Pitch limits")]
        [SerializeField] private float minPitch = -30f;
        [SerializeField] private float maxPitch = 60f;

        private PlayerControls _controls;
        private float _yaw;
        private float _pitch;

        private void Awake()
        {
            _controls = new PlayerControls();
            _controls.Enable();
        }

        private void OnDestroy()
        {
            _controls.Disable();
            _controls.Dispose();
        }

        private void LateUpdate()
        {
            Vector2 look = _controls.Player.Look.ReadValue<Vector2>();

            bool isGamepad = _controls.Player.Look.activeControl?.device is Gamepad;

            float multiplier = isGamepad
                ? gamepadSensitivity * Time.deltaTime
                : mouseSensitivity;

            _yaw += look.x * multiplier;
            _pitch -= look.y * multiplier;
            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

            transform.position = target.position + offset;
            transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        }
    }
}