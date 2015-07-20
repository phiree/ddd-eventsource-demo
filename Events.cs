using System;
using System.Collections.Generic;

namespace ddd_column
{
    public interface IEvent
    {
        Guid Id { get; }
    }

    public interface IEventHandler<in T>
        where T : IEvent
    {
        void Handle(T @event);
    }
}