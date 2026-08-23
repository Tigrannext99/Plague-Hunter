using UnityEngine;

namespace PlagueHunter.Combat
{
    /// <summary>
    /// Презентация урона: подсветка материала на попадание и заливка цветом смерти.
    /// Отделена от <see cref="Health"/>, чтобы модель здоровья не зависела от рендера.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Health))]
    public sealed class HealthFlash : MonoBehaviour
    {
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        [SerializeField] private Renderer _renderer;
        [SerializeField] private Color _flashColor = Color.white;
        [SerializeField] private Color _deadColor = new Color(0.15f, 0.15f, 0.15f, 1f);
        [SerializeField] private float _flashDuration = 0.12f;

        private Health _health;
        private MaterialPropertyBlock _block;
        private Color _baseColor;
        private float _timer;
        private bool _ready;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _ready = TryInitRenderer();

            if (!_ready) enabled = false;
        }

        private bool TryInitRenderer()
        {
            if (_renderer == null)
                _renderer = GetComponentInChildren<Renderer>();

            if (_renderer == null)
            {
                Debug.LogWarning($"[HealthFlash] на {name} не найден Renderer — подсветка выключена");
                return false;
            }

            Material material = _renderer.sharedMaterial;

            if (material == null || !material.HasProperty(BaseColorId))
            {
                Debug.LogWarning($"[HealthFlash] у материала на {name} нет свойства _BaseColor — подсветка выключена");
                return false;
            }

            _block = new MaterialPropertyBlock();
            _baseColor = material.GetColor(BaseColorId);

            return true;
        }

        private void OnEnable()
        {
            if (!_ready) return;

            _health.Damaged += OnDamaged;
            _health.Died += OnDied;

            _timer = 0f;
            SetColor(_baseColor);
        }

        private void OnDisable()
        {
            if (!_ready) return;

            _health.Damaged -= OnDamaged;
            _health.Died -= OnDied;
        }

        private void Update()
        {
            if (_timer <= 0f) return;

            _timer -= Time.deltaTime;

            if (_timer <= 0f)
            {
                SetColor(_baseColor);
                return;
            }

            float t = Mathf.Clamp01(_timer / _flashDuration);
            SetColor(Color.Lerp(_baseColor, _flashColor, t));
        }

        private void OnDamaged()
        {
            _timer = _flashDuration;
            SetColor(_flashColor);
        }

        private void OnDied()
        {
            _timer = 0f;
            SetColor(_deadColor);
        }

        private void SetColor(Color color)
        {
            _renderer.GetPropertyBlock(_block);
            _block.SetColor(BaseColorId, color);
            _renderer.SetPropertyBlock(_block);
        }
    }
}
