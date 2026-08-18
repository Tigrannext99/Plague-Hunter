using PlagueHunter.Core;
using UnityEngine;

namespace PlagueHunter.Player
{
    public sealed class AttackState : IState
    {
        private readonly PlayerRoot _player;

        private float _timer;

        public AttackState(PlayerRoot player) => _player = player;

        public void Enter()
        {
            _timer = _player.Config.AttackDuration;
            _player.Animator.SetTrigger(PlayerRoot.AttackHash);
        }

        public void Exit() { }

        public void Tick(float deltaTime)
        {
            _player.Locomotion.Tick(Vector2.zero, deltaTime);
            _player.Animator.SetFloat(PlayerRoot.SpeedHash, _player.Locomotion.CurrentSpeed);

            _timer -= deltaTime;

            if (_timer <= 0f)
                _player.Machine.SetState(_player.Idle);
        }
    }
}