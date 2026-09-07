using System.Collections.Generic;
using PlagueHunter.Enemy;
using UnityEngine;

namespace PlagueHunter.Gameplay
{
    /// <summary>
    /// Тестовая арена: держит в сцене заданное число живых врагов и подсыпает
    /// новых взамен убитых. Нужна, чтобы боёвку можно было щупать долго,
    /// не перезапуская сцену ради каждой пары ударов.
    /// </summary>
    public sealed class EnemySpawner : MonoBehaviour
    {
        private const int PlacementAttempts = 12;

        [Header("Кого спавним")]
        [SerializeField] private EnemyRoot _prefab;

        [Header("Сколько и как часто")]
        [Tooltip("Больше этого числа живых врагов одновременно в сцене не будет")]
        [SerializeField] private int _maxAlive = 10;
        [Tooltip("Пауза между появлениями")]
        [SerializeField] private float _spawnInterval = 0.5f;
        [Tooltip("Задержка перед самым первым врагом")]
        [SerializeField] private float _startDelay = 1f;

        [Header("Где спавним")]
        [Tooltip("Кольцо вокруг спавнера, в котором выбирается точка")]
        [SerializeField] private float _minRadius = 6f;
        [SerializeField] private float _maxRadius = 12f;
        [Tooltip("Ближе этого к игроку враг не появится")]
        [SerializeField] private float _minDistanceToPlayer = 6f;

        [Header("Трупы")]
        [Tooltip("Через сколько секунд после смерти убирать тело со сцены")]
        [SerializeField] private float _corpseLifetime = 4f;

        /// <summary>Спавнер сам ведёт учёт: труп ещё в сцене, но местом уже не занимает.</summary>
        private sealed class Spawned
        {
            public EnemyRoot Enemy;
            public bool Dead;
            public float DespawnAt;
        }

        private readonly List<Spawned> _spawned = new List<Spawned>();

        private Transform _target;
        private float _nextSpawnAt;
        private bool _composed;

        public int AliveCount { get; private set; }

        /// <summary>
        /// Цель приезжает снаружи тем же путём, что и у врагов в сцене:
        /// спавнер не ищет игрока сам, а получает его от GameplayRoot.
        /// </summary>
        public void Compose(Transform target)
        {
            _target = target;
            _nextSpawnAt = Time.time + _startDelay;
            _composed = true;

            if (_prefab == null)
                Debug.LogError("[EnemySpawner] поле Prefab пустое — спавнить нечего", this);
        }

        private void Update()
        {
            if (!_composed) return;

            TickCorpses();

            if (_prefab == null) return;

            TrySpawn();
        }

        /// <summary>
        /// Мёртвого перестаём считать живым сразу, а тело убираем позже:
        /// иначе замена приходила бы только после конца анимации смерти.
        /// </summary>
        private void TickCorpses()
        {
            int alive = 0;

            for (int i = _spawned.Count - 1; i >= 0; i--)
            {
                Spawned entry = _spawned[i];

                if (entry.Enemy == null)
                {
                    _spawned.RemoveAt(i);
                    continue;
                }

                if (!entry.Dead)
                {
                    if (!entry.Enemy.Health.IsDead)
                    {
                        alive++;
                        continue;
                    }

                    entry.Dead = true;
                    entry.DespawnAt = Time.time + _corpseLifetime;
                    continue;
                }

                if (Time.time < entry.DespawnAt) continue;

                Destroy(entry.Enemy.gameObject);
                _spawned.RemoveAt(i);
            }

            AliveCount = alive;
        }

        private void TrySpawn()
        {
            if (AliveCount >= _maxAlive) return;
            if (Time.time < _nextSpawnAt) return;

            // Точку не нашли — не жжём кулдаун, попробуем в следующем кадре.
            if (!TryFindPoint(out Vector3 point)) return;

            EnemyRoot enemy = Instantiate(_prefab, point, RotationTowardsTarget(point));
            enemy.Compose(_target);

            _spawned.Add(new Spawned { Enemy = enemy });
            _nextSpawnAt = Time.time + _spawnInterval;
        }

        private bool TryFindPoint(out Vector3 point)
        {
            for (int i = 0; i < PlacementAttempts; i++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2f);
                float radius = Random.Range(_minRadius, _maxRadius);

                point = transform.position
                        + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
                point.y = transform.position.y;

                if (IsFarEnoughFromPlayer(point)) return true;
            }

            point = Vector3.zero;
            return false;
        }

        private bool IsFarEnoughFromPlayer(Vector3 point)
        {
            if (_target == null) return true;

            Vector3 delta = _target.position - point;
            delta.y = 0f;

            return delta.sqrMagnitude >= _minDistanceToPlayer * _minDistanceToPlayer;
        }

        /// <summary>Новый враг смотрит на игрока, чтобы не появляться к нему спиной.</summary>
        private Quaternion RotationTowardsTarget(Vector3 point)
        {
            if (_target == null) return Quaternion.identity;

            Vector3 direction = _target.position - point;
            direction.y = 0f;

            return direction.sqrMagnitude < 0.0001f
                ? Quaternion.identity
                : Quaternion.LookRotation(direction);
        }

        private void OnValidate()
        {
            _maxAlive = Mathf.Max(1, _maxAlive);
            _spawnInterval = Mathf.Max(0f, _spawnInterval);
            _startDelay = Mathf.Max(0f, _startDelay);
            _minRadius = Mathf.Max(0f, _minRadius);
            _maxRadius = Mathf.Max(_maxRadius, _minRadius);
            _corpseLifetime = Mathf.Max(0f, _corpseLifetime);
            _minDistanceToPlayer = Mathf.Max(0f, _minDistanceToPlayer);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _minRadius);
            Gizmos.DrawWireSphere(transform.position, _maxRadius);
        }
    }
}
