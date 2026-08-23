using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlagueHunter.Core
{
    public sealed class Bootstrapper : MonoBehaviour
    {
        [SerializeField] private string _gameplaySceneName = "Gameplay";

        private GameplayInputReader _input;

        private async void Start()
        {
            _input = new GameplayInputReader();
            _input.Enable();

            try
            {
                await LoadGameplayAsync(destroyCancellationToken);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async void RequestRestart()
        {
            try
            {
                await RestartAsync(destroyCancellationToken);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async Awaitable RestartAsync(CancellationToken token)
        {
            var scene = SceneManager.GetSceneByName(_gameplaySceneName);

            if (scene.isLoaded)
            {
                var unload = SceneManager.UnloadSceneAsync(scene);

                while (unload is { isDone: false })
                    await Awaitable.NextFrameAsync(token);
            }

            await LoadGameplayAsync(token);
        }

        private async Awaitable LoadGameplayAsync(CancellationToken token)
        {
            var operation = SceneManager.LoadSceneAsync(_gameplaySceneName, LoadSceneMode.Additive);

            while (operation is { isDone: false })
                await Awaitable.NextFrameAsync(token);

            token.ThrowIfCancellationRequested();

            var scene = SceneManager.GetSceneByName(_gameplaySceneName);
            SceneManager.SetActiveScene(scene);

            var root = FindRoot(scene);

            if (root == null)
            {
                Debug.LogError($"[Bootstrapper] GameplayRoot not found in {_gameplaySceneName}");
                return;
            }

            root.Compose(_input, RequestRestart);
        }

        private static GameplayRoot FindRoot(Scene scene)
        {
            foreach (var go in scene.GetRootGameObjects())
            {
                var root = go.GetComponentInChildren<GameplayRoot>(true);
                if (root != null) return root;
            }

            return null;
        }

        private void OnDestroy()
        {
            _input?.Dispose();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}