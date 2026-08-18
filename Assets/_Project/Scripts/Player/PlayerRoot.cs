using PlagueHunter.Core;
using UnityEngine;

namespace PlagueHunter.Player
{
    public sealed class PlayerRoot : MonoBehaviour
    {
        [SerializeField] private CharacterController _controller;
        [SerializeField] private PlayerConfig _config;

        private GameplayInputReader _input;
        private PlayerLocomotion _locomotion;

        public void Compose(GameplayInputReader input, Transform cameraTransform)
        {
            _input = input;
            _locomotion = new PlayerLocomotion(_controller, cameraTransform, _config);
        }

        private void Update()
        {
            if (_locomotion == null) return;

            _locomotion.Tick(_input.Move, Time.deltaTime);
        }
    }
}