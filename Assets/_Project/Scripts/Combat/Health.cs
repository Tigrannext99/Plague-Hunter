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
        [SerializeField] bool _useFlash = true;
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
        public bool Invulnerable { get; set; }

        public event Action Died;

        void Awake()
        {
            _current = _maxHealth;

            if (!_useFlash) return;

            if (_renderer == null)
                _renderer = GetComponentInChildren<Renderer>();

            if (_renderer == null)
            {
                _useFlash = false;
                return;
            }

            Material material = _renderer.sharedMaterial;

            if (material == null || !material.HasProperty(BaseColorId))
            {
                Debug.LogWarning($"[Health] у материала на {name} нет свойства _BaseColor — флеш выключен");
                _useFlash = false;
                return;
            }

            _block = new MaterialPropertyBlock();
            _baseColor = material.GetColor(BaseColorId);
        }

        public void TakeDamage(float amount)
        {
            if (Invulnerable || IsDead || amount <= 0f) return;

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
        }

        void Update()
        {
            if (_flashTimer <= 0f) return;

            _flashTimer -= Time.deltaTime;

            if (_flashTimer <= 0f)
            {
                SetColor(_baseColor);
                return;
            }

            float t = Mathf.Clamp01(_flashTimer / _flashDuration);
            SetColor(Color.Lerp(_baseColor, _flashColor, t));
        }

        void SetColor(Color color)
        {
            if (!_useFlash || _renderer == null) return;

            _renderer.GetPropertyBlock(_block);
            _block.SetColor(BaseColorId, color);
            _renderer.SetPropertyBlock(_block);
        }
    }
}