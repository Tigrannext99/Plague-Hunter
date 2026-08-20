using PlagueHunter.Core;
using UnityEngine;

namespace PlagueHunter.Player
{
    public sealed class PlayerRoot : MonoBehaviour
    {
        public static readonly int SpeedHash = Animator.StringToHash("Speed");
        public static readonly int AttackHash = Animator.StringToHash("Attack");

        [SerializeField] private CharacterController _controller;
        [SerializeField] private Animator _animator;
        [SerializeField] private PlayerConfig _config;
        [SerializeField] private Transform _hitPoint;

        private readonly StateMachine _machine = new StateMachine();

        private Camera _camera;

        public GameplayInputReader Input { get; private set; }
        public PlayerLocomotion Locomotion { get; private set; }
        public Animator Animator => _animator;
        public PlayerConfig Config => _config;
        public StateMachine Machine => _machine;

        public IdleState Idle { get; private set; }
        public MoveState Move { get; private set; }
        public AttackState Attack { get; private set; }

        public Transform HitPoint => _hitPoint;
        public bool UseRootMotion { get; set; }

        public void Compose(GameplayInputReader input, Transform cameraTransform)
        {
            Input = input;
            _camera = cameraTransform.GetComponent<Camera>();
            Locomotion = new PlayerLocomotion(_controller, cameraTransform, _config);

            Idle = new IdleState(this);
            Move = new MoveState(this);
            Attack = new AttackState(this);

            _machine.SetState(Idle);
        }

        public Vector3 GetAimDirection()
        {
            return Input.IsGamepad ? AimFromStick() : AimFromCursor();
        }

        private Vector3 AimFromStick()
        {
            Vector3 direction = Locomotion.ToCameraSpace(Input.Move);
            return direction.sqrMagnitude < 0.0001f ? transform.forward : direction.normalized;
        }

        private Vector3 AimFromCursor()
        {
            if (_camera == null) return transform.forward;

            Ray ray = _camera.ScreenPointToRay(Input.Look);
            Plane ground = new Plane(Vector3.up, transform.position);

            if (!ground.Raycast(ray, out float distance)) return transform.forward;

            Vector3 flat = ray.GetPoint(distance) - transform.position;
            flat.y = 0f;

            return flat.sqrMagnitude < 0.0001f ? transform.forward : flat.normalized;
        }

        private void Update()
        {
            _machine.Tick(Time.deltaTime);
        }

        private void OnAnimatorMove()
        {
            if (!UseRootMotion) return;

            Vector3 motion = _animator.deltaPosition;
            motion.y = _config.GroundedGravity * Time.deltaTime;

            _controller.Move(motion);
        }

        private void OnDrawGizmosSelected()
        {
            if (_config == null || _hitPoint == null) return;

            Gizmos.color = Color.red;
            Gizmos.matrix = Matrix4x4.TRS(_hitPoint.position, _hitPoint.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, _config.HitBoxHalfExtents * 2f);
        }
    }
}