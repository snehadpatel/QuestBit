namespace Echo.Core.StateMachine
{
    public abstract class BaseState : IState
    {
        protected readonly StateMachine StateMachine;

        protected BaseState(StateMachine stateMachine)
        {
            StateMachine = stateMachine;
        }

        public virtual void Enter() { }
        public virtual void Update(float deltaTime) { }
        public virtual void FixedUpdate(float fixedDeltaTime) { }
        public virtual void Exit() { }
    }
}
