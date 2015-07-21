using ddd_column.Events;

namespace ddd_column.Framework
{
    public interface IEventBus
    {
        void Publish(IEvent @event);
    }
}