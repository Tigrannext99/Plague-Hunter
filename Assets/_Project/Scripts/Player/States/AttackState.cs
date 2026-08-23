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

        private AttackData _current;
        private int _index;
        private float _timer;

        public AttackState(PlayerRoot player) => _player = player;

        public void Enter()
        {
            _index = 0;
            _player.UseRootMotion = true;
            _player.Locomotion.Reset();
            _player.Animator.SetFloat(PlayerRoot.SpeedHash, 0f);

            if (!HasUsableCombo())
            {
                Debug.LogError("[AttackState] комбо не настроено — атака пропущена");
                _player.Machine.SetState(_player.Idle);
                return;
            }

            StartAttack(_player.Combo[0]);
        }

        public void Tick(float deltaTime)
        {
            float previous = _timer;
            _timer += deltaTime;

            if (IsInHitWindow(previous, _timer))
                ScanForTargets();

            if (_timer >= _current.HitEnd && _player.ConsumeDodgeBuffer())
            {
                _player.Machine.SetState(_player.Dodge);
                return;
            }

            if (IsInComboWindow(_timer) && HasNext && _player.ConsumeAttackBuffer())
            {
                _index++;
                StartAttack(_player.Combo[_index]);
                return;
            }

            // Проверка идёт после комбо-буфера: нажатая атака важнее зажатого стика,
            // иначе на ходу комбо было бы не собрать.
            if (_timer >= _current.CancelTime && _player.Input.Move.sqrMagnitude > 0.01f)
            {
                _player.Machine.SetState(_player.Move);
                return;
            }

            if (_timer >= _current.Duration)
                _player.Machine.SetState(_player.Idle);
        }

        public void Exit()
        {
            _player.UseRootMotion = false;
            _alreadyHit.Clear();
            _player.ConsumeAttackBuffer();

            _player.Animator.CrossFade(PlayerRoot.LocomotionHash, 0.001f, 0, 0f);
        }

        private bool HasNext => _index + 1 < _player.Combo.Length && _player.Combo[_index + 1] != null;

        private bool HasUsableCombo()
            => _player.Combo != null && _player.Combo.Length > 0 && _player.Combo[0] != null;

        private void StartAttack(AttackData attack)
        {
            _current = attack;
            _timer = 0f;
            _alreadyHit.Clear();

            _player.transform.rotation = Quaternion.LookRotation(_player.GetAimDirection());
            _player.Animator.CrossFade(attack.StateHash, 0.05f, 0, 0f);
        }

        private bool IsInHitWindow(float previous, float current)
            => current >= _current.HitStart && previous <= _current.HitEnd;

        private bool IsInComboWindow(float time)
            => time >= _current.ComboStart && time <= _current.ComboEnd;

        private void ScanForTargets()
        {
            Transform point = _player.HitPoint;

            int count = DamageScan.Box(
                point.position,
                _current.HitBoxHalfExtents,
                point.rotation,
                _player.Config.EnemyMask,
                _overlaps);

            DamageScan.Apply(_overlaps, count, _current.Damage, _alreadyHit);
        }
    }
}