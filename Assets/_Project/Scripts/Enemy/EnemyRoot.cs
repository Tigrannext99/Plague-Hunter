using PlagueHunter.Combat;
using PlagueHunter.Core;
using UnityEngine;

namespace PlagueHunter.Enemy
{
    [RequireComponent(typeof(Health))]
    public sealed class EnemyRoot : MonoBehaviour
    {
        public static readonly int SpeedHash = Animator.StringToHash("Speed");
        public static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
        public static readonly int HitHash = Animator.StringToHash("Hit");
        public static readonly int DeathHash = Animator.StringToHash("Death");

        [SerializeField] private CharacterController _controller;
        [SerializeField] private Animator _animator;
        [SerializeField] private EnemyConfig _config;
        [SerializeField] private EnemyAttackData _attackData;
        [SerializeField] private GroundTelegraph _telegraph;

        private readonly StateMachine _machine = new StateMachine();

        private Health _health;
        private Transform _target;
        private Health _targetHealth;
        private float _attackReadyAt;

        public Animator Animator => _animator;
        public EnemyConfig Config => _config;
        public EnemyAttackData AttackData => _attackData;
        public GroundTelegraph Telegraph => _telegraph;
        public Health Health => _health;
        public StateMachine Machine => _machine;
        public EnemyLocomotion Locomotion { get; private set; }

        public EnemyIdleState Idle { get; private set; }
        public EnemyChaseState Chase { get; private set; }
        public EnemyAttackState Attack { get; private set; }
        public EnemyHitState Hit { get; private set; }
        public EnemyDeathState Death { get; private set; }

        public bool UseRootMotion { get; set; }

        /// <summary>Цель жива и всё ещё существует.</summary>
        public bool HasTarget => _target != null && _targetHealth != null && !_targetHealth.IsDead;

        public bool AttackReady => Time.time >= _attackReadyAt;

        private void Awake() => _health = GetComponent<Health>();

        public void Compose(Transform target)
        {
            _target = target;
            _targetHealth = target != null ? target.GetComponent<Health>() : null;

            Locomotion = new EnemyLocomotion(_controller, _config);

            Idle = new EnemyIdleState(this);
            Chase = new EnemyChaseState(this);
            Attack = new EnemyAttackState(this);
            Hit = new EnemyHitState(this);
            Death = new EnemyDeathState(this);

            _health.Damaged += OnDamaged;
            _health.Died += OnDied;
            Death.Finished += OnDeathFinished;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            ValidateAnimator();
#endif

            _machine.SetState(Idle);
        }

        /// <summary>Плоское расстояние до цели: высота не должна влиять на дистанции боя.</summary>
        public float DistanceToTarget()
        {
            if (_target == null) return float.PositiveInfinity;

            Vector3 delta = _target.position - transform.position;
            delta.y = 0f;

            return delta.magnitude;
        }

        public Vector3 DirectionToTarget()
        {
            if (_target == null) return transform.forward;

            Vector3 delta = _target.position - transform.position;
            delta.y = 0f;

            return delta.sqrMagnitude < 0.0001f ? transform.forward : delta.normalized;
        }

        public bool SeesTarget() => HasTarget && DistanceToTarget() <= _config.AggroRadius;

        /// <summary>
        /// Порог потери больше порога захвата — между ними стейт не меняется,
        /// иначе враг мигал бы между покоем и погоней на самой границе.
        /// </summary>
        public bool LostTarget() => !HasTarget || DistanceToTarget() > _config.DeaggroRadius;

        public bool InAttackRange() => HasTarget && DistanceToTarget() <= _attackData.Range;

        public void StartAttackCooldown() => _attackReadyAt = Time.time + _attackData.Cooldown;

        /// <summary>Куда прилетит удар: точка перед врагом на дистанции Reach.</summary>
        public Vector3 StrikePoint()
        {
            Vector3 point = transform.position + transform.forward * _attackData.Reach;
            point.y = transform.position.y;

            return point;
        }

        public void FaceTargetInstantly()
        {
            if (!HasTarget) return;

            transform.rotation = Quaternion.LookRotation(DirectionToTarget());
        }

        private void OnDamaged()
        {
            // Атака врага — это обязательство: начал замах, довёл до конца.
            // Иначе игрок залочивает его стаггером и бой перестаёт быть боем.
            if (_machine.Current == Attack) return;

            _machine.SetState(Hit);
        }

        private void OnDied() => _machine.SetState(Death);

        /// <summary>Труп не должен толкать игрока и ловить удары — выключаем контроллер.</summary>
        private void OnDeathFinished() => _controller.enabled = false;

        private void Update() => _machine.Tick(Time.deltaTime);

        private void OnAnimatorMove()
        {
            if (!UseRootMotion) return;

            Vector3 motion = _animator.deltaPosition;
            motion.y = _config.GroundedGravity * Time.deltaTime;

            _controller.Move(motion);
        }

        private void OnDestroy()
        {
            if (Death != null)
                Death.Finished -= OnDeathFinished;

            if (_health == null) return;

            _health.Damaged -= OnDamaged;
            _health.Died -= OnDied;
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private void ValidateAnimator()
        {
            if (!_animator.HasState(0, LocomotionHash))
                Debug.LogError("[EnemyRoot] в аниматоре нет стейта 'Locomotion'", this);

            if (!_animator.HasState(0, HitHash))
                Debug.LogError("[EnemyRoot] в аниматоре нет стейта 'Hit'", this);

            if (!_animator.HasState(0, DeathHash))
                Debug.LogError("[EnemyRoot] в аниматоре нет стейта 'Death'", this);

            if (_attackData == null)
            {
                Debug.LogError("[EnemyRoot] поле Attack Data пустое", this);
                return;
            }

            if (!_animator.HasState(0, _attackData.StateHash))
                Debug.LogError(
                    $"[EnemyRoot] в аниматоре нет стейта '{_attackData.StateName}' (ассет {_attackData.name})",
                    this);
        }
#endif

        private void OnDrawGizmosSelected()
        {
            if (_config != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, _config.AggroRadius);

                Gizmos.color = new Color(1f, 0.6f, 0f, 0.5f);
                Gizmos.DrawWireSphere(transform.position, _config.DeaggroRadius);
            }

            if (_attackData == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(StrikePoint(), _attackData.HitRadius);

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, _attackData.Range);
        }
    }
}
