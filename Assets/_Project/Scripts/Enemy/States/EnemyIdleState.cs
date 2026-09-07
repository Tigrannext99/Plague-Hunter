using PlagueHunter.Core;
using UnityEngine;

namespace PlagueHunter.Enemy
{
    public sealed class EnemyIdleState : IState
    {
        private readonly EnemyRoot _enemy;

        public EnemyIdleState(EnemyRoot enemy) => _enemy = enemy;

        public void Enter() => _enemy.Locomotion.Reset();

        public void Exit() { }

        public void Tick(float deltaTime)
        {
            // Нулевое направление тормозит на месте, но продолжает прижимать к земле.
            _enemy.Locomotion.Tick(Vector3.zero, deltaTime);
            _enemy.Animator.SetFloat(EnemyRoot.SpeedHash, _enemy.Locomotion.CurrentSpeed);

            if (_enemy.SeesTarget())
                _enemy.Machine.SetState(_enemy.Chase);
        }
    }
}
