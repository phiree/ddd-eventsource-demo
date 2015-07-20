using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

namespace ddd_column
{
    public interface IEventStore
    {
        IEnumerable<IEvent> EventsFor(Guid id);

        void Save(Guid id, IEnumerable<IEvent> @events, int persistedVersion);
    }

    public class MemoryEventStore : IEventStore
    {
        private readonly IEventBus _eventBus;
        private readonly ConcurrentDictionary<Guid, AppendOnlyList<IEvent>> _events = new ConcurrentDictionary<Guid, AppendOnlyList<IEvent>>();

        public MemoryEventStore(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public IEnumerable<IEvent> EventsFor(Guid id)
        {
            AppendOnlyList<IEvent> events;
            if (_events.TryGetValue(id, out events))
                return events;

            return new List<IEvent>();
        }

        public void Save(Guid id, IEnumerable<IEvent> events, int persistedVersion)
        {
            AppendOnlyList<IEvent> items = _events.GetOrAdd(id, _ => new AppendOnlyList<IEvent>());

            // FIXME: Unit of work pattern
            foreach (var @event in events)
            {
                items.Append(++persistedVersion, @event);
                _eventBus.Publish(@event);
            }
        }

        private class AppendOnlyList<T> : IEnumerable<T>
        {
            private readonly List<T> _storage = new List<T>();

            public void Append(int version, T entity)
            {
                if (version <= _storage.Count)
                    throw new DBConcurrencyException();

                _storage.Add(entity);
            }

            public IEnumerator<T> GetEnumerator()
            {
                return _storage.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)_storage).GetEnumerator();
            }
        }
    }
}