using UnityEngine;

namespace PlagueHunter.Combat
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private int maxHealth = 100;

        private int _current;
        private Animator _animator;

        private static readonly int HitHash = Animator.StringToHash("Hit");

        public bool IsDead => _current <= 0;

        private void Awake()
        {
            _current = maxHealth;
            _animator = GetComponent<Animator>();
        }

        public void TakeDamage(int amount)
        {
            if (IsDead) return;

            _current -= amount;
            Debug.Log($"{name} HP: {_current}");

            if (_animator != null)
                _animator.SetTrigger(HitHash);

            if (_current <= 0)
                Debug.Log($"{name} died");
        }
    }
}