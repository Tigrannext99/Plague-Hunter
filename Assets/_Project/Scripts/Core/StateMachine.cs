namespace PlagueHunter.Core
{
    public sealed class StateMachine
    {
        public IState Current { get; private set; }

        public void SetState(IState next)
        {
            if (next == null || next == Current) return;

            Current?.Exit();
            Current = next;
            Current.Enter();
        }

        public void Tick(float deltaTime) => Current?.Tick(deltaTime);
    }
}