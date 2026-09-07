using PlagueHunter.Core;
using UnityEngine;

namespace PlagueHunter.Enemy
{
    public sealed class EnemyHitState : IState
    {
        private const float CrossFade = 0.05f;

        private readonly EnemyRoot _enemy;

        private float _timer;

        public EnemyHitState(EnemyRoot enemy) => _enemy = enemy;

        public void Enter()
        {
            _timer = 0f;

            _enemy.UseRootMotion = true;
            _enemy.Locomotion.Reset();
            _enemy.Animator.SetFloat(EnemyRoot.SpeedHash, 0f);
            _enemy.Animator.CrossFade(EnemyRoot.HitHash, CrossFade, 0, 0f);
        }

        public void Tick(float deltaTime)
        {
            _timer += deltaTime;

            if (_timer < _enemy.Config.HitStun) return;

            _enemy.Machine.SetState(_enemy.HasTarget ? _enemy.Chase : _enemy.Idle);
        }

        public void Exit()
        {
            _enemy.UseRootMotion = false;
            _enemy.Animator.CrossFade(EnemyRoot.LocomotionHash, 0.001f, 0, 0f);
        }
    }
}
