namespace PlagueHunter.Core
{
    public class StateMachine
    {
        private IState _current;

        public IState Current => _current;

        public void SetState(IState state)
        {
            _current?.Exit();
            _current = state;
            _current.Enter();
        }

        public void Tick(float deltaTime)
        {
            _current?.Tick(deltaTime);
        }
    }
}