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

        [Header("Input buffer")]
        [SerializeField] private float _attackBufferWindow = 0.2f;
        [SerializeField] private float _dodgeBufferWindow = 0.2f;

        [Header("Targets")]
        [SerializeField] private LayerMask _enemyMask;

        public float WalkSpeed => _walkSpeed;
        public float RunSpeed => _runSpeed;
        public float SpeedSharpness => _speedSharpness;
        public float RotationSharpness => _rotationSharpness;
        public float GroundedGravity => _groundedGravity;
        public float AttackBufferWindow => _attackBufferWindow;
        public float DodgeBufferWindow => _dodgeBufferWindow;
        public LayerMask EnemyMask => _enemyMask;

        private void OnValidate()
        {
            _attackBufferWindow = Mathf.Max(0f, _attackBufferWindow);
            _dodgeBufferWindow = Mathf.Max(0f, _dodgeBufferWindow);
        }
    }
}
