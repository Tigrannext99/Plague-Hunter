using UnityEngine;

namespace PlagueHunter.Combat
{
    public sealed class HitFeedback : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _hitParticle;
        [SerializeField] private int _poolSize = 8;

        private ParticleSystem[] _pool;
        private int _next;

        private void Awake()
        {
            if (_hitParticle == null)
            {
                Debug.LogWarning("[HitFeedback] партикл не назначен");
                return;
            }

            _pool = new ParticleSystem[_poolSize];

            for (int i = 0; i < _poolSize; i++)
            {
                _pool[i] = Instantiate(_hitParticle);
                _pool[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        public void PlayHit(Vector3 point, Vector3 direction)
        {
            SpawnParticle(point, direction);
        }

        private void SpawnParticle(Vector3 point, Vector3 direction)
        {
            if (_pool == null) return;

            ParticleSystem particle = _pool[_next];
            _next = (_next + 1) % _pool.Length;

            particle.transform.position = point;

            if (direction.sqrMagnitude > 0.0001f)
                particle.transform.rotation = Quaternion.LookRotation(direction);

            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particle.Play(true);
        }

        private void OnDestroy()
        {
            if (_pool == null) return;

            for (int i = 0; i < _pool.Length; i++)
                if (_pool[i] != null)
                    Destroy(_pool[i].gameObject);
        }
    }
}