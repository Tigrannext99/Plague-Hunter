using PlagueHunter.Core;
using UnityEngine;

namespace PlagueHunter.Player
{
    public sealed class MoveState : IState
    {
        private readonly PlayerRoot _player;

        public MoveState(PlayerRoot player) => _player = player;

        public void Enter() => _player.Input.AttackPressed += OnAttackPressed;

        public void Exit() => _player.Input.AttackPressed -= OnAttackPressed;

        public void Tick(float deltaTime)
        {
            _player.Locomotion.Tick(_player.Input.Move, deltaTime);
            _player.Animator.SetFloat(PlayerRoot.SpeedHash, _player.Locomotion.CurrentSpeed);

            if (_player.Input.Move.sqrMagnitude <= 0.01f)
                _player.Machine.SetState(_player.Idle);
        }

        private void OnAttackPressed() => _player.Machine.SetState(_player.Attack);
    }
}