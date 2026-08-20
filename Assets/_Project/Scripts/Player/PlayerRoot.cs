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

        public GameplayInputReader Input { get; private set; }
        public PlayerLocomotion Locomotion { get; private set; }
        public Animator Animator => _animator;
        public PlayerConfig Config => _config;
        public StateMachine Machine => _machine;

        public IdleState Idle { get; private set; }
        public MoveState Move { get; private set; }
        public AttackState Attack { get; private set; }

        public Transform HitPoint => _hitPoint;

        public void Compose(GameplayInputReader input, Transform cameraTransform)
        {
            Input = input;
            Locomotion = new PlayerLocomotion(_controller, cameraTransform, _config);

            Idle = new IdleState(this);
            Move = new MoveState(this);
            Attack = new AttackState(this);

            _machine.SetState(Idle);
        }

        private void Update()
        {
            _machine.Tick(Time.deltaTime);
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