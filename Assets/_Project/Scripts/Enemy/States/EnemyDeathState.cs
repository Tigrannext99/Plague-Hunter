using System;
using PlagueHunter.Core;
using UnityEngine;

namespace PlagueHunter.Enemy
{
    public sealed class EnemyDeathState : IState
    {
        private const float EnterTimeout = 1f;

        private readonly EnemyRoot _enemy;

        private float _timer;
        private bool _entered;
        private bool _finished;

        public event Action Finished;

        public EnemyDeathState(EnemyRoot enemy) => _enemy = enemy;

        public void Enter()
        {
            _timer = 0f;
            _entered = false;
            _finished = false;

            _enemy.UseRootMotion = true;
            _enemy.Locomotion.Reset();

            if (_enemy.Telegraph != null)
                _enemy.Telegraph.Hide();

            _enemy.Animator.SetFloat(EnemyRoot.SpeedHash, 0f);
            _enemy.Animator.CrossFade(EnemyRoot.DeathHash, 0.1f, 0, 0f);
        }

        public void Tick(float deltaTime)
        {
            if (_finished) return;

            _timer += deltaTime;

            if (_enemy.Animator.IsInTransition(0)) return;

            AnimatorStateInfo info = _enemy.Animator.GetCurrentAnimatorStateInfo(0);

            if (info.shortNameHash != EnemyRoot.DeathHash)
            {
                // Аниматор так и не дошёл до Death — не зависаем навсегда.
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
