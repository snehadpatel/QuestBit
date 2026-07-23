using System;

namespace Echo.Core.Events
{
    public class EventBinding<T> : IEventBinding<T> where T : IEvent
    {
        private Action<T> _onEvent = _ => { };
        private Action _onEventNoArgs = () => { };

        public Action<T> OnEvent
        {
            get => _onEvent;
            set => _onEvent = value ?? (_ => { });
        }

        public Action OnEventNoArgs
        {
            get => _onEventNoArgs;
            set => _onEventNoArgs = value ?? (() => { });
        }

        public EventBinding(Action<T> onEvent) => _onEvent = onEvent;
        public EventBinding(Action onEventNoArgs) => _onEventNoArgs = onEventNoArgs;

        public void Add(Action<T> action) => _onEvent += action;
        public void Remove(Action<T> action) => _onEvent -= action;

        public void Add(Action action) => _onEventNoArgs += action;
        public void Remove(Action action) => _onEventNoArgs -= action;
    }
}
