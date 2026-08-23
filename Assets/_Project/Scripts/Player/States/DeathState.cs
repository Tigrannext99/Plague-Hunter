using PlagueHunter.Core;

namespace PlagueHunter.Player
{
    public sealed class DeathState : IState
    {
        private readonly PlayerRoot _player;

        public DeathState(PlayerRoot player) => _player = player;

        public void Enter()
        {
            _player.UseRootMotion = true;
            _player.Locomotion.Reset();
            _player.Animator.SetFloat(PlayerRoot.SpeedHash, 0f);
            _player.Animator.CrossFade(PlayerRoot.DeathHash, 0.1f, 0, 0f);
        }

        public void Tick(float deltaTime) { }

        public void Exit() { }
    }
}