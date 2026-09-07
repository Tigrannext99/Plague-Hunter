using PlagueHunter.Core;
using UnityEngine;

namespace PlagueHunter.Enemy
{
    public sealed class EnemyChaseState : IState
    {
        private readonly EnemyRoot _enemy;

        public EnemyChaseState(EnemyRoot enemy) => _enemy = enemy;

        public void Enter() { }

        public void Exit() { }

        public void Tick(float deltaTime)
        {
            if (_enemy.LostTarget())
            {
                _enemy.Machine.SetState(_enemy.Idle);
                return;
            }

            Vector3 direction = _enemy.DirectionToTarget();

            if (_enemy.InAttackRange())
            {
                if (_enemy.AttackReady)
                {
                    _enemy.Machine.SetState(_enemy.Attack);
                    return;
                }

                // Дистанция набрана, но откат не вышел: враг держит позицию
                // и доворачивается, вместо того чтобы влезать игроку в модель.
                direction = Vector3.zero;
                _enemy.Locomotion.FaceTowards(_enemy.DirectionToTarget(), deltaTime);
            }

            _enemy.Locomotion.Tick(direction, deltaTime);
            _enemy.Animator.SetFloat(EnemyRoot.SpeedHash, _enemy.Locomotion.CurrentSpeed);
        }
    }
}
