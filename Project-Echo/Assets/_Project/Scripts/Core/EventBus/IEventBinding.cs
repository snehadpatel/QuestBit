using System;

namespace Echo.Core.Events
{
    public interface IEventBinding<T> where T : IEvent
    {
        Action<T> OnEvent { get; set; }
        Action OnEventNoArgs { get; set; }
    }
}
