using System;
using System.Collections.Generic;

namespace Echo.Core.StateMachine
{
    public class StateMachine
    {
        public IState CurrentState { get; private set; }
        public IState PreviousState { get; private set; }

        public event Action<IState, IState> OnStateChanged;

        public void ChangeState(IState newState)
        {
            if (newState == null || newState == CurrentState) return;

            CurrentState?.Exit();
            PreviousState = CurrentState;
            CurrentState = newState;
            CurrentState.Enter();

            OnStateChanged?.Invoke(PreviousState, CurrentState);
        }

        public void Update(float deltaTime)
        {
            CurrentState?.Update(deltaTime);
        }

        public void FixedUpdate(float fixedDeltaTime)
        {
            CurrentState?.FixedUpdate(fixedDeltaTime);
        }
    }
}
