using System;
using PlagueHunter.Core;
using PlagueHunter.Player;
using UnityEngine;

namespace PlagueHunter.Gameplay
{
    public sealed class GameplayRoot : MonoBehaviour
    {
        [SerializeField] private PlayerRoot _player;
        [SerializeField] private Camera _camera;
        [SerializeField] private float _restartDelay = 2.5f;

        private GameplayInputReader _input;
        private Action _restart;
        private bool _restarting;

        public void Compose(GameplayInputReader input, Action restart)
        {
            _input = input;
            _restart = restart;

            _input.LockOnPressed += OnLockOnPressed;

            _player.Compose(input, ResolveCamera());
            _player.Health.Died += OnPlayerDied;

            Debug.Log("[GameplayRoot] Composed");
        }

        private Camera ResolveCamera()
        {
            if (_camera != null) return _camera;

            _camera = Camera.main;

            Debug.LogWarning("[GameplayRoot] поле Camera пустое, взята Camera.main — назначь ссылку в инспекторе");

            return _camera;
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
