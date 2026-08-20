using PlagueHunter.Core;

namespace PlagueHunter.Player
{
    public sealed class MoveState : IState
    {
        private readonly PlayerRoot _player;

        public MoveState(PlayerRoot player) => _player = player;

        public void Enter() { }

        public void Exit() { }

        public void Tick(float deltaTime)
        {
            _player.Locomotion.Tick(_player.Input.Move, deltaTime);
            _player.Animator.SetFloat(PlayerRoot.SpeedHash, _player.Locomotion.CurrentSpeed);

            if (_player.ConsumeAttackBuffer())
            {
                _player.Machine.SetState(_player.Attack);
                return;
            }

            if (_player.Input.Move.sqrMagnitude <= 0.01f)
                _player.Machine.SetState(_player.Idle);
        }
    }
}