using UnityEngine;

namespace PlagueHunter.Combat
{
    public sealed class GroundTelegraph : MonoBehaviour
    {
        [SerializeField] private Transform _outline;
        [SerializeField] private Transform _fill;
        [SerializeField] private float _groundOffset = 0.02f;

        private float _diameter;

        private void Awake() => Hide();

        public void Begin(Vector3 position, float radius)
        {
            _diameter = radius * 2f;

            position.y += _groundOffset;
            transform.position = position;

            _outline.localScale = new Vector3(_diameter, _diameter, 1f);
            _fill.localScale = Vector3.zero;

            SetVisible(true);
        }

        public void SetProgress(float t)
        {
            float size = _diameter * Mathf.Clamp01(t);
            _fill.localScale = new Vector3(size, size, 1f);
        }

        public void Hide() => SetVisible(false);

        private void SetVisible(bool value)
        {
            _outline.gameObject.SetActive(value);
            _fill.gameObject.SetActive(value);
        }
    }
}