using Echo.Core.Events;
using NUnit.Framework;

namespace Echo.Tests.CoreTests
{
    public class EventBusTests
    {
        private struct TestEvent : IEvent
        {
            public int Value;
        }

        [SetUp]
        public void SetUp()
        {
            EventBus<TestEvent>.Clear();
        }

        [Test]
        public void EventBus_Raise_InvokesRegisteredBinding()
        {
            int receivedValue = 0;
            var binding = new EventBinding<TestEvent>(evt => receivedValue = evt.Value);
            EventBus<TestEvent>.Register(binding);

            EventBus<TestEvent>.Raise(new TestEvent { Value = 42 });

            Assert.AreEqual(42, receivedValue);
        }

        [Test]
        public void EventBus_Deregister_StopsEventInvocation()
        {
            int callCount = 0;
            var binding = new EventBinding<TestEvent>(() => callCount++);
            EventBus<TestEvent>.Register(binding);
            EventBus<TestEvent>.Deregister(binding);

            EventBus<TestEvent>.Raise(new TestEvent { Value = 100 });

            Assert.AreEqual(0, callCount);
        }
    }
}
