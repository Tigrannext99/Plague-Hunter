using UnityEngine;

namespace PlagueHunter.Enemy
{
    /// <summary>
    /// Движение врага по прямой к цели: тот же контракт, что у игрока,
    /// только направление приходит не с ввода, а из стейта.
    /// </summary>
    public sealed class EnemyLocomotion
    {
        private const float DirectionDeadZone = 0.0001f;

        private readonly CharacterController _controller;
        private readonly Transform _transform;
        private readonly EnemyConfig _config;

        private Vector3 _currentVelocity;

        public float CurrentSpeed => _currentVelocity.magnitude;

        public EnemyLocomotion(CharacterController controller, EnemyConfig config)
        {
            _controller = controller;
            _transform = controller.transform;
            _config = config;
        }

        /// <param name="direction">
        /// Направление в мировых координатах, длина не важна.
        /// Нулевое — враг тормозит на месте, но остаётся прижат к земле.
        /// </param>
        public void Tick(Vector3 direction, float deltaTime)
        {
            direction = Flatten(direction);

            Vector3 targetVelocity = direction * _config.MoveSpeed;

            float speedFactor = 1f - Mathf.Exp(-_config.SpeedSharpness * deltaTime);
            _currentVelocity = Vector3.Lerp(_currentVelocity, targetVelocity, speedFactor);

            Vector3 motion = _currentVelocity;
            motion.y = _config.GroundedGravity;

            _controller.Move(motion * deltaTime);

            FaceTowards(direction, deltaTime);
        }

        /// <summary>
        /// Доворот без шага — нужен в замахе, когда двигаться уже нельзя,
        /// но упускать цель из прицела не хочется.
        /// </summary>
        public void FaceTowards(Vector3 direction, float deltaTime)
        {
            direction = Flatten(direction);

            if (direction.sqrMagnitude < DirectionDeadZone) return;

            Quaternion target = Quaternion.LookRotation(direction);
            float factor = 1f - Mathf.Exp(-_config.RotationSharpness * deltaTime);

            _transform.rotation = Quaternion.Slerp(_transform.rotation, target, factor);
        }

        public void Reset() => _currentVelocity = Vector3.zero;

        private static Vector3 Flatten(Vector3 vector)
        {
            vector.y = 0f;

            return vector.sqrMagnitude < DirectionDeadZone ? Vector3.zero : vector.normalized;
        }
    }
}
