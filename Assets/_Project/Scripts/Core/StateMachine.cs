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

        /// <summary>
        /// Перезапуск текущего стейта полным циклом Exit → Enter.
        /// Нужен там, где стейт входит сам в себя (например повторный додж),
        /// чтобы флаги стейта сбрасывались тем же путём, что и при обычном переходе.
        /// </summary>
        public void ReEnter()
        {
            if (Current == null) return;

            Current.Exit();
            Current.Enter();
        }

        public void Tick(float deltaTime) => Current?.Tick(deltaTime);
    }
}
