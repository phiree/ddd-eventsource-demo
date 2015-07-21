using System;

namespace ddd_column.Framework
{
    public interface IEventSourcedRepository<T>
        where T : AggregateRoot
    {
        T Get(Guid id);
        void Save(T aggregateRoot);
    }
}