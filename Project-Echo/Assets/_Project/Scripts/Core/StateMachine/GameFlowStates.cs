using System;

namespace Echo.Core.StateMachine
{
    public class BootState : BaseState
    {
        public BootState(StateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            UnityEngine.Debug.Log("[GameFlow] Entering BootState. Initializing core services...");
        }
    }

    public class MainMenuState : BaseState
    {
        public MainMenuState(StateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            UnityEngine.Debug.Log("[GameFlow] Entering MainMenuState.");
        }
    }

    public class LobbyState : BaseState
    {
        public LobbyState(StateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            UnityEngine.Debug.Log("[GameFlow] Entering LobbyState (Matchmaking & Session Prep).");
        }
    }

    public class SessionState : BaseState
    {
        public SessionState(StateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            UnityEngine.Debug.Log("[GameFlow] Entering SessionState (Active Gameplay).");
        }
    }

    public class PauseState : BaseState
    {
        public PauseState(StateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            UnityEngine.Debug.Log("[GameFlow] Entering PauseState.");
        }
    }
}
