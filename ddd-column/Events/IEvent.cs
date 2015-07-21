using System;

namespace ddd_column.Events
{
    public interface IEvent
    {
        Guid Id { get; }
    }
}