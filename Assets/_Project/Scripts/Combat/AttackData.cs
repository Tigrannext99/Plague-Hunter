using UnityEngine;

namespace PlagueHunter.Combat
{
    [CreateAssetMenu(menuName = "PlagueHunter/Attack Data")]
    public sealed class AttackData : ScriptableObject
    {
        [SerializeField] private string _stateName = "Attack_1";
        [SerializeField] private float _duration = 1f;

        [Header("Hit window (seconds)")]
        [SerializeField] private float _hitStart = 0.25f;
        [SerializeField] private float _hitEnd = 0.35f;

        [Header("Combo window (seconds)")]
        [SerializeField] private float _comboStart = 0.4f;
        [SerializeField] private float _comboEnd = 0.8f;

        [Header("Damage")]
        [SerializeField] private float _damage = 25f;
        [SerializeField] private Vector3 _hitBoxHalfExtents = new Vector3(0.15f, 0.15f, 0.5f);

        private int _stateHash = -1;

        public string StateName => _stateName;

        public int StateHash
        {
            get
            {
                if (_stateHash == -1) _stateHash = Animator.StringToHash(_stateName);
                return _stateHash;
            }
        }

        public float Duration => _duration;
        public float HitStart => _hitStart;
        public float HitEnd => _hitEnd;
        public float ComboStart => _comboStart;
        public float ComboEnd => _comboEnd;
        public float Damage => _damage;
        public Vector3 HitBoxHalfExtents => _hitBoxHalfExtents;

        private void OnValidate()
        {
            _stateHash = -1;

            _hitEnd = Mathf.Max(_hitEnd, _hitStart);
            _comboEnd = Mathf.Max(_comboEnd, _comboStart);
            _duration = Mathf.Max(_duration, _comboEnd);
            _duration = Mathf.Max(_duration, _hitEnd);
        }
    }
}