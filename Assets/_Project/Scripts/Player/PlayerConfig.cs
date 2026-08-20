using UnityEngine;

namespace PlagueHunter.Player
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "PlagueHunter/Player Config")]
    public sealed class PlayerConfig : ScriptableObject
    {
        [Header("Movement")]
        [SerializeField] private float _walkSpeed = 2f;
        [SerializeField] private float _runSpeed = 5.5f;

        [Header("Smoothing")]
        [SerializeField] private float _speedSharpness = 12f;
        [SerializeField] private float _rotationSharpness = 14f;

        [Header("Gravity")]
        [SerializeField] private float _groundedGravity = -2f;

        [Header("Attack")]
        [SerializeField] private float _attackDuration = 1.633f;
        [SerializeField] private float _attackDamage = 25f;
        [SerializeField] private float _hitStart = 0.35f;
        [SerializeField] private float _hitEnd = 0.7f;
        [SerializeField] private Vector3 _hitBoxHalfExtents = new Vector3(0.15f, 0.15f, 0.5f);
        [SerializeField] private LayerMask _enemyMask;

        public float WalkSpeed => _walkSpeed;
        public float RunSpeed => _runSpeed;
        public float SpeedSharpness => _speedSharpness;
        public float RotationSharpness => _rotationSharpness;
        public float GroundedGravity => _groundedGravity;
        public float AttackDuration => _attackDuration;
        public float AttackDamage => _attackDamage;
        public float HitStart => _hitStart;
        public float HitEnd => _hitEnd;
        public Vector3 HitBoxHalfExtents => _hitBoxHalfExtents;
        public LayerMask EnemyMask => _enemyMask;

        private void OnValidate()
        {
            _hitStart = Mathf.Clamp(_hitStart, 0f, _attackDuration);
            _hitEnd = Mathf.Clamp(_hitEnd, _hitStart, _attackDuration);
        }
    }
}