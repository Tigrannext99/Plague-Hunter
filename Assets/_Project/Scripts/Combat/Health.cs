using System;
using UnityEngine;

namespace PlagueHunter.Combat
{
    /// <summary>
    /// Модель здоровья. Только данные и события — никакой презентации.
    /// Визуальная реакция на урон живёт в <see cref="HealthFlash"/>.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private float _maxHealth = 100f;

        private float _current;

        public float Current => _current;
        public float Max => _maxHealth;
        public float Normalized => _maxHealth <= 0f ? 0f : _current / _maxHealth;
        public bool IsDead => _current <= 0f;
        public bool Invulnerable { get; set; }

        public event Action Died;
        public event Action Damaged;
        public event Action<float> Changed;

        private void Awake() => ResetHealth();

        public void ResetHealth()
        {
            _current = _maxHealth;
            Invulnerable = false;

            Changed?.Invoke(Normalized);
        }

        public void TakeDamage(float amount)
        {
            if (Invulnerable || IsDead || amount <= 0f) return;

            _current = Mathf.Max(0f, _current - amount);

            Changed?.Invoke(Normalized);

            if (IsDead)
            {
                Died?.Invoke();
                return;
            }

            Damaged?.Invoke();
        }
    }
}
