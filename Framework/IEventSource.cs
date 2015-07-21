using ddd_column.Events;

namespace ddd_column.Framework
{
    public interface IEventSource<in T> where T : IEvent
    {
        void Apply(T @event);
    }
}