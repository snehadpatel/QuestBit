using System;
using System.Collections.Generic;

namespace Echo.Core.Events
{
    public static class EventBus<T> where T : IEvent
    {
        private static readonly HashSet<IEventBinding<T>> _bindings = new HashSet<IEventBinding<T>>();

        public static void Register(IEventBinding<T> binding)
        {
            if (binding != null)
            {
                _bindings.Add(binding);
            }
        }

        public static void Deregister(IEventBinding<T> binding)
        {
            if (binding != null)
            {
                _bindings.Remove(binding);
            }
        }

        public static void Raise(T @event)
        {
            foreach (var binding in _bindings)
            {
                binding.OnEvent?.Invoke(@event);
                binding.OnEventNoArgs?.Invoke();
            }
        }

        public static void Clear()
        {
            _bindings.Clear();
        }
    }
}
