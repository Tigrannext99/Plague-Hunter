using UnityEngine;

namespace PlagueHunter.Combat
{
    /// <summary>
    /// Общая часть любой атаки: анимационный стейт, длительность,
    /// окно нанесения урона и сам урон.
    /// Форма попадания и правила отмены зависят от носителя и живут в наследниках.
    /// </summary>
    public abstract class AttackData : ScriptableObject
    {
        [SerializeField] private string _stateName = "Attack_1";
        [SerializeField] private float _duration = 1f;

        [Header("Hit window (seconds)")]
        [SerializeField] private float _hitStart = 0.25f;
        [SerializeField] private float _hitEnd = 0.35f;

        [Header("Damage")]
        [SerializeField] private float _damage = 25f;

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
        public float Damage => _damage;

        /// <summary>
        /// Растягивает атаку так, чтобы она вмещала заданный момент времени.
        /// Наследники зовут это до base.OnValidate() для своих окон.
        /// </summary>
        protected void RequireDuration(float seconds) => _duration = Mathf.Max(_duration, seconds);

        protected virtual void OnValidate()
        {
            _stateHash = -1;

            _hitEnd = Mathf.Max(_hitEnd, _hitStart);
            _duration = Mathf.Max(_duration, _hitEnd);
            _damage = Mathf.Max(_damage, 0f);
        }
    }
}
