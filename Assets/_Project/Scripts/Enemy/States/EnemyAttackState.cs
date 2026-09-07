using System.Collections.Generic;
using PlagueHunter.Combat;
using PlagueHunter.Core;
using UnityEngine;

namespace PlagueHunter.Enemy
{
    public sealed class EnemyAttackState : IState
    {
        private const float CrossFade = 0.05f;

        private readonly EnemyRoot _enemy;
        private readonly Collider[] _overlaps = new Collider[8];
        private readonly HashSet<IDamageable> _alreadyHit = new HashSet<IDamageable>();

        private EnemyAttackData _attack;
        private Vector3 _strikePoint;
        private float _timer;
        private bool _telegraphShown;

        public EnemyAttackState(EnemyRoot enemy) => _enemy = enemy;

        public void Enter()
        {
            _attack = _enemy.AttackData;
            _timer = 0f;
            _alreadyHit.Clear();

            _enemy.UseRootMotion = true;
            _enemy.Locomotion.Reset();
            _enemy.Animator.SetFloat(EnemyRoot.SpeedHash, 0f);

            // Прицел берётся один раз на входе: дальше точка удара стоит на месте,
            // чтобы телеграф не врал и из него можно было выйти доджем.
            _enemy.FaceTargetInstantly();
            _strikePoint = _enemy.StrikePoint();

            _telegraphShown = _attack.ShowTelegraph && _enemy.Telegraph != null;

            if (_telegraphShown)
            {
                _enemy.Telegraph.Begin(_strikePoint, _attack.HitRadius);
                _enemy.Telegraph.SetProgress(0f);
            }

            _enemy.Animator.CrossFade(_attack.StateHash, CrossFade, 0, 0f);
        }

        public void Tick(float deltaTime)
        {
            float previous = _timer;
            _timer += deltaTime;

            TickTelegraph();

            if (IsInHitWindow(previous, _timer))
                Strike();

            if (_timer >= _attack.Duration)
                _enemy.Machine.SetState(_enemy.LostTarget() ? _enemy.Idle : _enemy.Chase);
        }

        public void Exit()
        {
            _enemy.UseRootMotion = false;
            _alreadyHit.Clear();

            HideTelegraph();
            _enemy.StartAttackCooldown();
            _enemy.Animator.CrossFade(EnemyRoot.LocomotionHash, 0.001f, 0, 0f);
        }

        private void TickTelegraph()
        {
            if (!_telegraphShown) return;

            if (_timer >= _attack.Windup)
            {
                // Замах кончился — маркер убираем ровно в момент удара.
                HideTelegraph();
                return;
            }

            _enemy.Telegraph.SetProgress(_timer / _attack.Windup);
        }

        private void HideTelegraph()
        {
            if (!_telegraphShown) return;

            _enemy.Telegraph.Hide();
            _telegraphShown = false;
        }

        /// <summary>
        /// Окно открыто на всех кадрах между HitStart и HitEnd,
        /// повторные попадания режет <see cref="_alreadyHit"/>.
        /// </summary>
        private bool IsInHitWindow(float previous, float current)
            => current >= _attack.HitStart && previous <= _attack.HitEnd;

        private void Strike()
        {
            int count = DamageScan.Sphere(
                _strikePoint,
                _attack.HitRadius,
                _enemy.Config.TargetMask,
                _overlaps);

            DamageScan.Apply(_overlaps, count, _attack.Damage, _alreadyHit);
        }
    }
}
