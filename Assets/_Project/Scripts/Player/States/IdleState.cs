using PlagueHunter.Core;
using UnityEngine;

namespace PlagueHunter.Player
{
    public sealed class IdleState : IState
    {
        private readonly PlayerRoot _player;

        public IdleState(PlayerRoot player) => _player = player;

        public void Enter() { }

        public void Exit() { }

        public void Tick(float deltaTime)
        {
            _player.Locomotion.Tick(Vector2.zero, deltaTime);
            _player.Animator.SetFloat(PlayerRoot.SpeedHash, _player.Locomotion.CurrentSpeed);

            if (_player.ConsumeDodgeBuffer())
            {
                _player.Machine.SetState(_player.Dodge);
                return;
            }

            if (_player.ConsumeAttackBuffer())
            {
                _player.Machine.SetState(_player.Attack);
                return;
            }

            if (_player.Input.Move.sqrMagnitude > 0.01f)
                _player.Machine.SetState(_player.Move);
        }
    }
}