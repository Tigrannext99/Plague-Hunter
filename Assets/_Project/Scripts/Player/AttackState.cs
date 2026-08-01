using PlagueHunter.Combat;
using PlagueHunter.Core;
using UnityEngine;

namespace PlagueHunter.Player
{
    public class AttackState : IState
    {
        private const float InputBuffer = 0.2f;

        private readonly PlayerContext _ctx;
        private readonly ComboData _combo;
        private readonly int _index;
        private readonly AttackData _attack;

        private float _timer;
        private bool _hitDone;
        private bool _buffered;

        public AttackState(PlayerContext ctx, ComboData combo, int index)
        {
            _ctx = ctx;
            _combo = combo;
            _index = index;
            _attack = combo.Get(index);
        }

        public void Enter()
        {
            _timer = 0f;
            _hitDone = false;
            _buffered = false;

            _ctx.CurrentSpeed = 0f;
            _ctx.PlayerController.UseRootMotion = true;
            _ctx.Animator.CrossFadeInFixedTime(_attack.animationStateName, _attack.crossFadeDuration);
        }

        public void Exit()
        {
            _ctx.PlayerController.UseRootMotion = false;
        }

        public void Tick(float deltaTime)
        {
            ApplyTurn(deltaTime);

            _timer += deltaTime;

            if (_ctx.Input.AttackPressed && _timer >= _attack.comboStart - InputBuffer)
                _buffered = true;

            if (!_hitDone && _timer >= _attack.hitStart && _timer <= _attack.hitEnd)
            {
                DealDamage();
                _hitDone = true;
            }

            if (TryAdvance()) return;

            ApplyGravity(deltaTime);
            _ctx.Controller.Move(Vector3.up * _ctx.VerticalVelocity * deltaTime);

            if (_timer >= _attack.duration)
                _ctx.StateMachine.SetState(new LocomotionState(_ctx));
        }

        private bool TryAdvance()
        {
            if (!_buffered) return false;
            if (!_combo.HasNext(_index)) return false;
            if (_timer < _attack.comboStart) return false;
            if (_timer > _attack.comboEnd) return false;

            _ctx.StateMachine.SetState(new AttackState(_ctx, _combo, _index + 1));
            return true;
        }

        private Vector3 CameraRelative(Vector2 input)
        {
            Transform cam = _ctx.CameraTransform;

            Vector3 forward = cam.forward;
            Vector3 right = cam.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            return forward * input.y + right * input.x;
        }

        private void ApplyTurn(float deltaTime)
        {
            if (_timer > _ctx.Config.attackTurnDuration) return;

            Vector2 raw = Vector2.ClampMagnitude(_ctx.Input.Move, 1f);
            if (raw.sqrMagnitude < 0.01f) return;

            Vector3 dir = CameraRelative(raw.normalized);
            if (dir.sqrMagnitude < 0.0001f) return;

            Quaternion target = Quaternion.LookRotation(dir, Vector3.up);

            _ctx.Transform.rotation = Quaternion.RotateTowards(
                _ctx.Transform.rotation,
                target,
                _ctx.Config.attackTurnSpeed * deltaTime);
        }

        private void DealDamage()
        {
            Vector3 center = _ctx.Transform.TransformPoint(_attack.hitboxOffset);

            Collider[] hits = Physics.OverlapBox(
                center,
                _attack.hitboxSize * 0.5f,
                _ctx.Transform.rotation,
                _ctx.Config.enemyLayers);

            bool hitAnything = false;

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(_attack.damage);
                    hitAnything = true;
                }
            }

            if (hitAnything)
                _ctx.HitStop.Play();
        }

        private void ApplyGravity(float deltaTime)
        {
            var cfg = _ctx.Config;

            if (_ctx.Controller.isGrounded && _ctx.VerticalVelocity < 0f)
                _ctx.VerticalVelocity = cfg.groundedStick;
            else
                _ctx.VerticalVelocity += cfg.gravity * deltaTime;
        }
    }
}