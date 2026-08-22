using UnityEngine;

namespace PlagueHunter.Core
{
    public sealed class CameraImpulse : MonoBehaviour
    {
        [SerializeField] private float _strength = 0.12f;
        [SerializeField] private float _duration = 0.15f;
        [SerializeField] private float _frequency = 28f;

        private Vector3 _basePosition;
        private float _currentStrength;
        private float _currentDuration;
        private float _timer;
        private float _seed;

        private void Awake()
        {
            _basePosition = transform.localPosition;
            _seed = Random.value * 100f;
        }

        public void Play(float scale = 1f)
        {
            if (scale <= 0f) return;

            float strength = _strength * scale;

            if (_timer > 0f && strength < _currentStrength) return;

            _currentStrength = strength;
            _currentDuration = _duration;
            _timer = _duration;
        }

        private void LateUpdate()
        {
            if (_timer <= 0f) return;

            _timer -= Time.deltaTime;

            if (_timer <= 0f)
            {
                transform.localPosition = _basePosition;
                return;
            }

            float falloff = _timer / _currentDuration;
            float amount = _currentStrength * falloff * falloff;
            float time = (Time.time + _seed) * _frequency;

            Vector3 offset = new Vector3(
                (Mathf.PerlinNoise(time, 0f) - 0.5f) * 2f,
                (Mathf.PerlinNoise(0f, time) - 0.5f) * 2f,
                0f);

            transform.localPosition = _basePosition + offset * amount;
        }
    }
}