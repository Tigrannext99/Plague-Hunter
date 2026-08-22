using UnityEngine;

namespace PlagueHunter.Combat
{
    [RequireComponent(typeof(Health))]
    public sealed class DummyAttacker : MonoBehaviour
    {
        private enum Phase
        {
            Idle,
            Windup,
            Recover
        }

        [Header("Target")]
        [SerializeField] private LayerMask _targetMask;
        [SerializeField] private float _aggroRadius = 6f;

        [Header("Attack")]
        [SerializeField] private float _reach = 1.6f;
        [SerializeField] private float _attackRadius = 1.8f;
        [SerializeField] private float _windup = 1.2f;
        [SerializeField] private float _recover = 1.5f;
        [SerializeField] private float _damage = 20f;

        [Header("Telegraph")]
        [SerializeField] private GroundTelegraph _telegraph;

        private readonly Collider[] _overlaps = new Collider[8];

        private Health _health;
        private Phase _phase;
        private float _timer;
        private Vector3 _strikePoint;

        private void Awake() => _health = GetComponent<Health>();

        private void Update()
        {
            if (_health.IsDead)
            {
                if (_phase != Phase.Idle)
                {
                    _telegraph.Hide();
                    _phase = Phase.Idle;
                }

                return;
            }

            switch (_phase)
            {
                case Phase.Idle:
                    TickIdle();
                    break;

                case Phase.Windup:
                    TickWindup(Time.deltaTime);
                    break;

                case Phase.Recover:
                    TickRecover(Time.deltaTime);
                    break;
            }
        }

        private void TickIdle()
        {
            Transform target = FindTarget();

            if (target == null) return;

            FaceTarget(target);

            _strikePoint = transform.position + transform.forward * _reach;
            _strikePoint.y = transform.position.y;

            _telegraph.Begin(_strikePoint, _attackRadius);
            _telegraph.SetProgress(0f);

            _timer = 0f;
            _phase = Phase.Windup;
        }

        private void TickWindup(float deltaTime)
        {
            _timer += deltaTime;
            _telegraph.SetProgress(_timer / _windup);

            if (_timer < _windup) return;

            Strike();

            _telegraph.Hide();
            _timer = 0f;
            _phase = Phase.Recover;
        }

        private void TickRecover(float deltaTime)
        {
            _timer += deltaTime;

            if (_timer >= _recover)
                _phase = Phase.Idle;
        }

        private void Strike()
        {
            int count = Physics.OverlapSphereNonAlloc(
                _strikePoint,
                _attackRadius,
                _overlaps,
                _targetMask,
                QueryTriggerInteraction.Ignore);

            for (int i = 0; i < count; i++)
            {
                if (_overlaps[i].TryGetComponent(out IDamageable damageable))
                    damageable.TakeDamage(_damage);
            }
        }

        private Transform FindTarget()
        {
            int count = Physics.OverlapSphereNonAlloc(
                transform.position,
                _aggroRadius,
                _overlaps,
                _targetMask,
                QueryTriggerInteraction.Ignore);

            return count > 0 ? _overlaps[0].transform : null;
        }

        private void FaceTarget(Transform target)
        {
            Vector3 direction = target.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.0001f) return;

            transform.rotation = Quaternion.LookRotation(direction);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _aggroRadius);

            Gizmos.color = Color.red;
            Vector3 point = transform.position + transform.forward * _reach;
            Gizmos.DrawWireSphere(point, _attackRadius);
        }
    }
}