using UnityEngine;

namespace PlagueHunter.Enemy
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "PlagueHunter/Enemy Config")]
    public sealed class EnemyConfig : ScriptableObject
    {
        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 3f;

        [Header("Smoothing")]
        [SerializeField] private float _speedSharpness = 10f;
        [SerializeField] private float _rotationSharpness = 8f;

        [Header("Gravity")]
        [SerializeField] private float _groundedGravity = -2f;

        [Header("Aggro")]
        [Tooltip("Дистанция, с которой враг замечает цель")]
        [SerializeField] private float _aggroRadius = 8f;
        [Tooltip("Дистанция, на которой враг теряет уже замеченную цель")]
        [SerializeField] private float _deaggroRadius = 14f;

        [Header("Targets")]
        [SerializeField] private LayerMask _targetMask;

        public float MoveSpeed => _moveSpeed;
        public float SpeedSharpness => _speedSharpness;
        public float RotationSharpness => _rotationSharpness;
        public float GroundedGravity => _groundedGravity;
        public float AggroRadius => _aggroRadius;
        public float DeaggroRadius => _deaggroRadius;
        public LayerMask TargetMask => _targetMask;

        private void OnValidate()
        {
            _moveSpeed = Mathf.Max(_moveSpeed, 0f);
            _aggroRadius = Mathf.Max(_aggroRadius, 0f);

            // Гистерезис: терять цель нужно дальше, чем замечать,
            // иначе враг дёргается между погоней и покоем на границе радиуса.
            _deaggroRadius = Mathf.Max(_deaggroRadius, _aggroRadius);
        }
    }
}
