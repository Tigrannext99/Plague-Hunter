using System.Collections.Generic;
using PlagueHunter.Combat;
using PlagueHunter.Core;
using UnityEngine;

namespace PlagueHunter.Player
{
    public sealed class AttackState : IState
    {
        private readonly PlayerRoot _player;
        private readonly Collider[] _overlaps = new Collider[16];
        private readonly HashSet<IDamageable> _alreadyHit = new HashSet<IDamageable>();

        private float _timer;

        public AttackState(PlayerRoot player) => _player = player;

        public void Enter()
        {
            _player.transform.rotation = Quaternion.LookRotation(_player.GetAimDirection());

            _timer = 0f;
            _alreadyHit.Clear();
            _player.UseRootMotion = true;

            _player.Animator.SetFloat(PlayerRoot.SpeedHash, 0f);
            _player.Animator.SetTrigger(PlayerRoot.AttackHash);
        }

        public void Tick(float deltaTime)
        {
            float previous = _timer;
            _timer += deltaTime;

            if (IsInHitWindow(previous, _timer))
                ScanForTargets();

            if (_timer >= _player.Config.AttackDuration)
                _player.Machine.SetState(_player.Idle);
        }

        public void Exit()
        {
            _player.UseRootMotion = false;
            _alreadyHit.Clear();
        }

        private bool IsInHitWindow(float previous, float current)
        {
            PlayerConfig config = _player.Config;
            return current >= config.HitStart && previous <= config.HitEnd;
        }

        private void ScanForTargets()
        {
            Transform point = _player.HitPoint;

            int count = Physics.OverlapBoxNonAlloc(
                point.position,
                _player.Config.HitBoxHalfExtents,
                _overlaps,
                point.rotation,
                _player.Config.EnemyMask,
                QueryTriggerInteraction.Ignore);

            for (int i = 0; i < count; i++)
            {
                if (!_overlaps[i].TryGetComponent(out IDamageable damageable)) continue;
                if (!_alreadyHit.Add(damageable)) continue;

                damageable.TakeDamage(_player.Config.AttackDamage);
            }
        }
    }
}