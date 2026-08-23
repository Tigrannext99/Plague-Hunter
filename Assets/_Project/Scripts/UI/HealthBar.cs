using PlagueHunter.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace PlagueHunter.UI
{
    public sealed class HealthBar : MonoBehaviour
    {
        [SerializeField] private Health _health;
        [SerializeField] private Image _fill;
        [SerializeField] private float _sharpness = 10f;

        private float _target = 1f;

        private void Start()
        {
            if (_health == null)
            {
                Debug.LogError("[HealthBar] поле Health пустое");
                enabled = false;
                return;
            }

            _health.Changed += OnChanged;

            _target = _health.Normalized;
            _fill.fillAmount = _target;
        }

        private void Update()
        {
            if (Mathf.Approximately(_fill.fillAmount, _target)) return;

            float factor = 1f - Mathf.Exp(-_sharpness * Time.deltaTime);
            _fill.fillAmount = Mathf.Lerp(_fill.fillAmount, _target, factor);
        }

        private void OnDestroy()
        {
            if (_health != null)
                _health.Changed -= OnChanged;
        }

        private void OnChanged(float normalized) => _target = normalized;
    }
}