using UnityEngine;

namespace PlagueHunter.Player
{
    public sealed class PlayerLocomotion
    {
        private readonly CharacterController _controller;
        private readonly Transform _transform;
        private readonly Transform _cameraTransform;
        private readonly PlayerConfig _config;

        private Vector3 _currentVelocity;

        public float CurrentSpeed => _currentVelocity.magnitude;

        public PlayerLocomotion(
            CharacterController controller,
            Transform cameraTransform,
            PlayerConfig config)
        {
            _controller = controller;
            _transform = controller.transform;
            _cameraTransform = cameraTransform;
            _config = config;
        }

        public void Tick(Vector2 moveInput, float deltaTime)
        {
            Vector3 direction = ToCameraSpace(moveInput);
            Vector3 targetVelocity = direction * _config.RunSpeed;

            float speedFactor = 1f - Mathf.Exp(-_config.SpeedSharpness * deltaTime);
            _currentVelocity = Vector3.Lerp(_currentVelocity, targetVelocity, speedFactor);

            Vector3 motion = _currentVelocity;
            motion.y = _config.GroundedGravity;

            _controller.Move(motion * deltaTime);

            Rotate(direction, deltaTime);
        }

        public Vector3 ToCameraSpace(Vector2 moveInput)
        {
            Vector2 clamped = Vector2.ClampMagnitude(moveInput, 1f);

            if (clamped.sqrMagnitude < 0.0001f)
                return Vector3.zero;

            Vector3 forward = Flatten(_cameraTransform.forward);
            Vector3 right = Flatten(_cameraTransform.right);

            return (forward * clamped.y + right * clamped.x).normalized * clamped.magnitude;
        }

        private static Vector3 Flatten(Vector3 vector)
        {
            vector.y = 0f;
            return vector.normalized;
        }

        private void Rotate(Vector3 direction, float deltaTime)
        {
            if (direction.sqrMagnitude < 0.0001f)
                return;

            Quaternion target = Quaternion.LookRotation(direction);
            float factor = 1f - Mathf.Exp(-_config.RotationSharpness * deltaTime);

            _transform.rotation = Quaternion.Slerp(_transform.rotation, target, factor);
        }

        public void Reset() => _currentVelocity = Vector3.zero;
    }
}