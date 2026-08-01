using PlagueHunter.Core;
using UnityEngine;

namespace PlagueHunter.Player
{
    public class LocomotionState : IState
    {
        private static readonly int SpeedHash = Animator.StringToHash("Speed");

        private readonly PlayerContext _ctx;

        public LocomotionState(PlayerContext ctx) => _ctx = ctx;

        public void Enter() { }
        public void Exit() { }

        public void Tick(float deltaTime)
        {
            var cfg = _ctx.Config;

            Vector2 raw = Vector2.ClampMagnitude(_ctx.Input.Move, 1f);
            Vector3 dir = CameraRelative(raw);

            float maxSpeed = _ctx.Input.RunHeld ? cfg.runSpeed : cfg.walkSpeed;
            float targetSpeed = dir.magnitude * maxSpeed;

            _ctx.CurrentSpeed = Mathf.Lerp(
                _ctx.CurrentSpeed,
                targetSpeed,
                1f - Mathf.Exp(-cfg.speedSharpness * deltaTime));

            if (dir.sqrMagnitude > 0.0001f)
            {
                Quaternion target = Quaternion.LookRotation(dir);
                _ctx.Transform.rotation = Quaternion.Slerp(
                    _ctx.Transform.rotation,
                    target,
                    1f - Mathf.Exp(-cfg.rotationSharpness * deltaTime));
            }

            ApplyGravity(deltaTime);

            Vector3 motion = _ctx.Transform.forward * _ctx.CurrentSpeed
                             + Vector3.up * _ctx.VerticalVelocity;
            _ctx.Controller.Move(motion * deltaTime);

            _ctx.Animator.SetFloat(SpeedHash, _ctx.CurrentSpeed, cfg.animDampTime, deltaTime);
        }

        private Vector3 CameraRelative(Vector2 input)
        {
            Vector3 forward = _ctx.CameraTransform.forward;
            Vector3 right = _ctx.CameraTransform.right;

            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            return forward * input.y + right * input.x;
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