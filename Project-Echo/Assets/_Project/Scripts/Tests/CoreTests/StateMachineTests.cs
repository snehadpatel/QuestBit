using Echo.Core.StateMachine;
using NUnit.Framework;

namespace Echo.Tests.CoreTests
{
    public class StateMachineTests
    {
        private class MockState : BaseState
        {
            public bool Entered { get; private set; }
            public bool Exited { get; private set; }

            public MockState(StateMachine stateMachine) : base(stateMachine) { }

            public override void Enter() => Entered = true;
            public override void Exit() => Exited = true;
        }

        [Test]
        public void StateMachine_ChangeState_TransitionsCorrectly()
        {
            var sm = new StateMachine();
            var stateA = new MockState(sm);
            var stateB = new MockState(sm);

            sm.ChangeState(stateA);
            Assert.IsTrue(stateA.Entered);
            Assert.AreSame(stateA, sm.CurrentState);

            sm.ChangeState(stateB);
            Assert.IsTrue(stateA.Exited);
            Assert.IsTrue(stateB.Entered);
            Assert.AreSame(stateB, sm.CurrentState);
            Assert.AreSame(stateA, sm.PreviousState);
        }
    }
}
