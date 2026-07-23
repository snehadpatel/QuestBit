namespace Echo.Core.StateMachine
{
    public interface IState
    {
        void Enter();
        void Update(float deltaTime);
        void FixedUpdate(float fixedDeltaTime);
        void Exit();
    }
}
