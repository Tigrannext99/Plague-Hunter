using PlagueHunter.Combat;
using PlagueHunter.Core;
using UnityEngine;

namespace PlagueHunter.Player
{
    public sealed class PlayerRoot : MonoBehaviour
    {
        public static readonly int SpeedHash = Animator.StringToHash("Speed");
        public static readonly int LocomotionHash = Animator.StringToHash("Locomotion");

        [SerializeField] private CharacterController _controller;
        [SerializeField] private Animator _animator;
        [SerializeField] private PlayerConfig _config;
        [SerializeField] private ComboData _combo;
        [SerializeField] private Transform _hitPoint;

        private const float AttackBufferWindow = 0.2f;

        private readonly StateMachine _machine = new StateMachine();

        private Camera _camera;
        private float _attackPressedAt = float.NegativeInfinity;

        public GameplayInputReader Input { get; private set; }
        public PlayerLocomotion Locomotion { get; private set; }
        public Animator Animator => _animator;
        public PlayerConfig Config => _config;
        public ComboData Combo => _combo;
        public StateMachine Machine => _machine;

        public IdleState Idle { get; private set; }
        public MoveState Move { get; private set; }
        public AttackState Attack { get; private set; }

        public Transform HitPoint => _hitPoint;
        public bool UseRootMotion { get; set; }

        public void Compose(GameplayInputReader input, Transform cameraTransform)
        {
            Input = input;
            Input.AttackPressed += OnAttackPressed;

            _camera = cameraTransform.GetComponent<Camera>();
            Locomotion = new PlayerLocomotion(_controller, cameraTransform, _config);

            Idle = new IdleState(this);
            Move = new MoveState(this);
            Attack = new AttackState(this);

            ValidateAnimator();

            _machine.SetState(Idle);
        }

        public bool ConsumeAttackBuffer()
        {
            if (Time.time - _attackPressedAt > AttackBufferWindow) return false;

            _attackPressedAt = float.NegativeInfinity;
            return true;
        }

        public Vector3 GetAimDirection()
        {
            return Input.IsGamepad ? AimFromStick() : AimFromCursor();
        }

        private void ValidateAnimator()
        {
            if (!_animator.HasState(0, LocomotionHash))
                Debug.LogError("[PlayerRoot] в аниматоре нет стейта 'Locomotion'");

            if (_combo == null)
            {
                Debug.LogError("[PlayerRoot] поле Combo пустое");
                return;
            }

            for (int i = 0; i < _combo.Length; i++)
            {
                AttackData attack = _combo[i];

                if (attack == null)
                {
                    Debug.LogError($"[PlayerRoot] в комбо пустой слот {i}");
                    continue;
                }

                if (!_animator.HasState(0, attack.StateHash))
                    Debug.LogError($"[PlayerRoot] в аниматоре нет стейта '{attack.StateName}' (ассет {attack.name})");
            }
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

        private void OnAttackPressed() => _attackPressedAt = Time.time;

        private void Update()
        {
            _machine.Tick(Time.deltaTime);
        }

        private void OnDestroy()
        {
            if (Input != null)
                Input.AttackPressed -= OnAttackPressed;
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
            if (_combo == null || _combo.Length == 0 || _combo[0] == null || _hitPoint == null) return;

            Gizmos.color = Color.red;
            Gizmos.matrix = Matrix4x4.TRS(_hitPoint.position, _hitPoint.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, _combo[0].HitBoxHalfExtents * 2f);
        }
    }
}