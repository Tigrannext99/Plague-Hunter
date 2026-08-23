using PlagueHunter.Combat;
using PlagueHunter.Core;
using UnityEngine;

namespace PlagueHunter.Player
{
    public sealed class DodgeState : IState
    {
        private readonly PlayerRoot _player;

        private DodgeData _data;
        private float _timer;
        private bool _iFramesOn;

        public DodgeState(PlayerRoot player) => _player = player;

        public void Enter()
        {
            _data = _player.DodgeData;
            _timer = 0f;
            _iFramesOn = false;

            _player.UseRootMotion = true;
            _player.Locomotion.Reset();
            _player.Animator.SetFloat(PlayerRoot.SpeedHash, 0f);

            _player.transform.rotation = Quaternion.LookRotation(GetDodgeDirection());
            _player.Animator.CrossFade(_data.StateHash, _data.CrossFade, 0, 0f);
        }

        public void Tick(float deltaTime)
        {
            _timer += deltaTime;

            UpdateIFrames();

            if (_timer < _data.CancelTime) return;

            if (_player.ConsumeDodgeBuffer())
            {
                _player.Machine.ReEnter();
                return;
            }

            if (_player.ConsumeAttackBuffer())
            {
                _player.Machine.SetState(_player.Attack);
                return;
            }

            bool hasMoveInput = _player.Input.Move.sqrMagnitude > 0.01f;

            // После CancelTime ввод движения обрывает хвост анимации.
            // Без этого управление возвращается только на Duration и додж ощущается вязким.
            if (hasMoveInput)
            {
                _player.Machine.SetState(_player.Move);
                return;
            }

            if (_timer < _data.Duration) return;

            _player.Machine.SetState(_player.Idle);
        }

        public void Exit()
        {
            _player.UseRootMotion = false;
            _player.SetInvulnerable(false);
            _player.ConsumeDodgeBuffer();

            _player.Animator.CrossFade(PlayerRoot.LocomotionHash, 0.05f, 0, 0f);
        }

        private void UpdateIFrames()
        {
            bool active = _timer >= _data.IFramesStart && _timer <= _data.IFramesEnd;

            if (active == _iFramesOn) return;

            _iFramesOn = active;
            _player.SetInvulnerable(active);
        }

        private Vector3 GetDodgeDirection()
        {
            Vector3 direction = _player.Locomotion.ToCameraSpace(_player.Input.Move);

            return direction.sqrMagnitude < 0.0001f
                ? _player.transform.forward
                : direction.normalized;
        }
    }
}