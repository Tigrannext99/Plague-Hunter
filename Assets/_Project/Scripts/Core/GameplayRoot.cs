using System;
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

            _player.Compose(input, Camera.main.transform);
            _player.Health.Died += OnPlayerDied;

            Debug.Log("[GameplayRoot] Composed");
        }

        private void OnPlayerDied()
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
            if (_player != null && _player.Health != null)
                _player.Health.Died -= OnPlayerDied;

            if (_input == null) return;

            _input.LockOnPressed -= OnLockOnPressed;
        }

        private void OnLockOnPressed() => Debug.Log("LockOn");
    }
}