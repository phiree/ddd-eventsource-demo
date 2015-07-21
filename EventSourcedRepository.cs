using System;
using System.Collections;
using System.Collections.Generic;

namespace ddd_column
{
    public interface IEventSourcedRepository<T> : IRepository<T>
        where T : EventSourcedAggregateRoot
    {
    }

    public class EventSourcedRepository<T> : IEventSourcedRepository<T>
        where T : EventSourcedAggregateRoot
    {
        private readonly Func<Guid, IEnumerable<IEvent>, T> _create;
        private readonly IEventStore _eventStore;

        public EventSourcedRepository(Func<Guid, IEnumerable<IEvent>, T> create, IEventStore eventStore)
        {
            _create = create;
            _eventStore = eventStore;
        }

        public T Get(Guid id)
        {
            IEnumerable<IEvent> events = _eventStore.EventsFor(id);
            return _create(id, events);
        }

        public void Save(T aggregateRoot)
        {
            IReadOnlyList<IEvent> events = aggregateRoot.UncommittedUncommitedEvents;
            _eventStore.Save(aggregateRoot.Id, events, aggregateRoot.CommittedVersion);
            aggregateRoot.Commit(aggregateRoot.CommittedVersion + events.Count);
        }
    }
}