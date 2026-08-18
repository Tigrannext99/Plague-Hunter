using PlagueHunter.Core;
using UnityEngine;

namespace PlagueHunter.Player
{
    public sealed class PlayerRoot : MonoBehaviour
    {
        private static readonly int SpeedHash = Animator.StringToHash("Speed");

        [SerializeField] private CharacterController _controller;
        [SerializeField] private Animator _animator;
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
            _animator.SetFloat(SpeedHash, _locomotion.CurrentSpeed, Time.deltaTime, 0.1f);
        }
    }
}