using System;
using UnityEngine;

namespace PlagueHunter.Combat
{
    [DisallowMultipleComponent]
    public sealed class Health : MonoBehaviour, IDamageable
    {
        static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        [Header("Health")]
        [SerializeField] float _maxHealth = 100f;

        [Header("Hit Flash")]
        [SerializeField] Renderer _renderer;
        [SerializeField] Color _flashColor = Color.white;
        [SerializeField] Color _deadColor = new Color(0.15f, 0.15f, 0.15f, 1f);
        [SerializeField] float _flashDuration = 0.12f;

        MaterialPropertyBlock _block;
        Color _baseColor;
        float _flashTimer;
        float _current;

        public float Current => _current;
        public float Max => _maxHealth;
        public bool IsDead => _current <= 0f;

        public event Action Died;

        void Awake()
        {
            _current = _maxHealth;
            _block = new MaterialPropertyBlock();
            

            if (_renderer == null)
                _renderer = GetComponentInChildren<Renderer>();

            if (_renderer != null)
                _baseColor = _renderer.sharedMaterial.GetColor(BaseColorId);

            Debug.Log(_renderer.sharedMaterial.shader.name);
        }

        public void TakeDamage(float amount)
        {
            if (IsDead || amount <= 0f) return;

            _current = Mathf.Max(0f, _current - amount);

            if (IsDead)
            {
                _flashTimer = 0f;
                SetColor(_deadColor);
                Died?.Invoke();
                return;
            }

            _flashTimer = _flashDuration;
            SetColor(_flashColor);

            Debug.Log($"hit {name} {_current}");
        }

        void Update()
        {
            if (_flashTimer <= 0f) return;

            _flashTimer -= Time.deltaTime;

            float t = Mathf.Clamp01(_flashTimer / _flashDuration);
            SetColor(Color.Lerp(_baseColor, _flashColor, t));
        }

        void SetColor(Color color)
        {
            if (_renderer == null) return;

            _renderer.GetPropertyBlock(_block);
            _block.SetColor(BaseColorId, color);
            _renderer.SetPropertyBlock(_block);
        }
    }
}