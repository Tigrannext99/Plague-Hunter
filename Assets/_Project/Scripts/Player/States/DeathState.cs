using System;
using PlagueHunter.Core;
using UnityEngine;

namespace PlagueHunter.Player
{
    public sealed class DeathState : IState
    {
        private const float EnterTimeout = 1f;

        private readonly PlayerRoot _player;

        private float _timer;
        private bool _entered;
        private bool _finished;

        public event Action Finished;

        public DeathState(PlayerRoot player) => _player = player;

        public void Enter()
        {
            _timer = 0f;
            _entered = false;
            _finished = false;

            _player.UseRootMotion = true;
            _player.Locomotion.Reset();
            _player.Animator.SetFloat(PlayerRoot.SpeedHash, 0f);
            _player.Animator.CrossFade(PlayerRoot.DeathHash, 0.1f, 0, 0f);
        }

        public void Tick(float deltaTime)
        {
            if (_finished) return;

            _timer += deltaTime;

            if (_player.Animator.IsInTransition(0)) return;

            AnimatorStateInfo info = _player.Animator.GetCurrentAnimatorStateInfo(0);

            if (info.shortNameHash != PlayerRoot.DeathHash)
            {
                if (_entered || _timer >= EnterTimeout)
                    Complete();

                return;
            }

            _entered = true;

            if (info.normalizedTime < 1f) return;

            Complete();
        }

        public void Exit() { }

        private void Complete()
        {
            _finished = true;
            Finished?.Invoke();
        }
    }
}