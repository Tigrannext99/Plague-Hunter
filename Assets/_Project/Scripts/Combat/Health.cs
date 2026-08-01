using System.Collections;
using UnityEngine;

namespace PlagueHunter.Combat
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private Color hitColor = Color.red;
        [SerializeField] private float flashDuration = 0.12f;

        private int _current;
        private Renderer _renderer;
        private Color _baseColor;
        private Coroutine _flash;

        public bool IsDead => _current <= 0;

        private void Awake()
        {
            _current = maxHealth;
            _renderer = GetComponentInChildren<Renderer>();

            if (_renderer != null)
                _baseColor = _renderer.material.color;
        }

        public void TakeDamage(int amount)
        {
            if (IsDead) return;

            _current -= amount;
            Debug.Log($"{name} HP: {_current}");

            if (_renderer != null)
            {
                if (_flash != null) StopCoroutine(_flash);
                _flash = StartCoroutine(Flash());
            }

            if (_current <= 0)
                Debug.Log($"{name} died");
        }

        private IEnumerator Flash()
        {
            _renderer.material.color = hitColor;
            yield return new WaitForSeconds(flashDuration);
            _renderer.material.color = _baseColor;
            _flash = null;
        }
    }
}