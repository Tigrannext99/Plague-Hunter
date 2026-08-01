using PlagueHunter.Combat;
using PlagueHunter.Core;
using UnityEngine;

namespace PlagueHunter.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(HitStop))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerConfig config;
        [SerializeField] private Animator animator;

        private CharacterController _controller;
        private StateMachine _fsm;
        private PlayerContext _ctx;
        private PlayerInputReader _input;
        private HitStop _hitStop;

        public bool UseRootMotion { get; set; }

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _hitStop = GetComponent<HitStop>();
            _input = new PlayerInputReader();
            _fsm = new StateMachine();

            _ctx = new PlayerContext(
                transform,
                _controller,
                animator,
                _input,
                config,
                Camera.main.transform,
                _fsm,
                this,
                _hitStop);

            _fsm.SetState(new LocomotionState(_ctx));
        }

        private void Update() => _fsm.Tick(Time.deltaTime);

        private void OnDestroy() => _input.Dispose();

        private void OnAnimatorMove()
        {
            if (!UseRootMotion) return;
            _controller.Move(animator.deltaPosition);
        }

        private void OnDrawGizmosSelected()
        {
            if (config == null || config.combos == null || config.combos.Length == 0) return;
            if (config.combos[0] == null) return;

            var attack = config.combos[0].Get(0);
            if (attack == null) return;

            Gizmos.color = Color.red;
            Gizmos.matrix = Matrix4x4.TRS(
                transform.TransformPoint(attack.hitboxOffset),
                transform.rotation,
                Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, attack.hitboxSize);
        }
    }
}