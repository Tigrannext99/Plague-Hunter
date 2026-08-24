using UnityEngine;

namespace PlagueHunter.Player
{
    public sealed class PlayerLocomotion
    {
        private const float InputDeadZone = 0.0001f;

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

        /// <summary>
        /// Отклонение стика задаёт только направление, не скорость:
        /// любой ненулевой ввод даёт полный ход, как на клавиатуре.
        /// </summary>
        public Vector3 ToCameraSpace(Vector2 moveInput)
        {
            if (moveInput.sqrMagnitude < InputDeadZone)
                return Vector3.zero;

            Vector3 forward = Flatten(_cameraTransform.forward);
            Vector3 right = Flatten(_cameraTransform.right);

            return (forward * moveInput.y + right * moveInput.x).normalized;
        }

        private static Vector3 Flatten(Vector3 vector)
        {
            vector.y = 0f;
            return vector.normalized;
        }

        private void Rotate(Vector3 direction, float deltaTime)
        {
            if (direction.sqrMagnitude < InputDeadZone)
                return;

            Quaternion target = Quaternion.LookRotation(direction);
            float factor = 1f - Mathf.Exp(-_config.RotationSharpness * deltaTime);

            _transform.rotation = Quaternion.Slerp(_transform.rotation, target, factor);
        }

        public void Reset() => _currentVelocity = Vector3.zero;
    }
}