using System;
using PlagueHunter.Enemy;
using PlagueHunter.Player;
using UnityEngine;

namespace PlagueHunter.Core
{
    public sealed class GameplayRoot : MonoBehaviour
    {
        [SerializeField] private PlayerRoot _player;
        [SerializeField] private float _restartDelay = 2.5f;

        private GameplayInputReader _input;
        private Action _restart;
        private bool _restarting;

        public void Compose(GameplayInputReader input, Action restart)
        {
            _input = input;
            _restart = restart;

            _input.LockOnPressed += OnLockOnPressed;

            _player.Compose(input, Camera.main);
            _player.Death.Finished += OnDeathAnimationFinished;

            int enemies = ComposeEnemies();

            Debug.Log($"[GameplayRoot] Composed, врагов в сцене: {enemies}");
        }

        /// <summary>
        /// Врагов ищем по сцене, а не тянем ссылками в инспекторе:
        /// их количество меняется от прогона к прогону, а забытая ссылка
        /// дала бы молча стоящего болвана вместо ошибки.
        /// </summary>
        private int ComposeEnemies()
        {
            EnemyRoot[] enemies = FindObjectsByType<EnemyRoot>(FindObjectsSortMode.None);

            foreach (EnemyRoot enemy in enemies)
                enemy.Compose(_player.transform);

            return enemies.Length;
        }

        private void OnDeathAnimationFinished()
        {
            if (_restarting) return;

            _restarting = true;
            RestartAfterDelay();
        }

        private async void RestartAfterDelay()
        {
            try
            {
                await Awaitable.WaitForSecondsAsync(_restartDelay, destroyCancellationToken);
                _restart?.Invoke();
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void OnDestroy()
        {
            if (_player != null && _player.Death != null)
                _player.Death.Finished -= OnDeathAnimationFinished;

            if (_input == null) return;

            _input.LockOnPressed -= OnLockOnPressed;
        }

        private void OnLockOnPressed() => Debug.Log("LockOn");
    }
}