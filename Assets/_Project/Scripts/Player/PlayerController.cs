using PlagueHunter.Core;
using UnityEngine;

namespace PlagueHunter.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerConfig config;
        [SerializeField] private Animator animator;

        private StateMachine _fsm;
        private PlayerContext _ctx;
        private PlayerInputReader _input;

        private void Awake()
        {
            _input = new PlayerInputReader();
            _fsm = new StateMachine();

            _ctx = new PlayerContext(
                transform,
                GetComponent<CharacterController>(),
                animator,
                _input,
                config,
                Camera.main.transform,
                _fsm);

            _fsm.SetState(new LocomotionState(_ctx));
        }

        private void Update() => _fsm.Tick(Time.deltaTime);

        private void OnDestroy() => _input.Dispose();
    }
}