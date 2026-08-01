using PlagueHunter.Combat;
using PlagueHunter.Core;
using UnityEngine;

namespace PlagueHunter.Player
{
    public class AttackState : IState
    {
        private readonly PlayerContext _ctx;
        private readonly AttackData _attack;

        private float _timer;
        private bool _hitDone;

        public AttackState(PlayerContext ctx, AttackData attack)
        {
            _ctx = ctx;
            _attack = attack;
        }

        public void Enter()
        {
            _timer = 0f;
            _hitDone = false;

            _ctx.CurrentSpeed = 0f;
            _ctx.Animator.CrossFadeInFixedTime(_attack.animationStateName, _attack.crossFadeDuration);
        }

        public void Exit() { }

        public void Tick(float deltaTime)
        {
            _timer += deltaTime;

            if (!_hitDone && _timer >= _attack.hitStart && _timer <= _attack.hitEnd)
            {
                DealDamage();
                _hitDone = true;
            }

            ApplyGravity(deltaTime);
            _ctx.Controller.Move(Vector3.up * _ctx.VerticalVelocity * deltaTime);

            if (_timer >= _attack.duration)
                _ctx.StateMachine.SetState(new LocomotionState(_ctx));
        }

        private void DealDamage()
        {
            Vector3 center = _ctx.Transform.TransformPoint(_attack.hitboxOffset);

            Collider[] hits = Physics.OverlapBox(
                center,
                _attack.hitboxSize * 0.5f,
                _ctx.Transform.rotation,
                _ctx.Config.enemyLayers);

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].TryGetComponent(out IDamageable damageable))
                    damageable.TakeDamage(_attack.damage);
            }
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