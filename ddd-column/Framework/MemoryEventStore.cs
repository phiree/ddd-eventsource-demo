using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ddd_column.Events;
using Newtonsoft.Json;

namespace ddd_column.Framework
{
    public class MemoryEventStore : IEventStore
    {
        private readonly IEventBus _eventBus;
        private readonly ConcurrentDictionary<Guid, AppendOnlyList<string>> _events = new ConcurrentDictionary<Guid, AppendOnlyList<string>>();

        public MemoryEventStore(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public IEnumerable<IEvent> EventsFor(Guid id, int fromVersion)
        {
            AppendOnlyList<string> serializedEvents;
            if (_events.TryGetValue(id, out serializedEvents))
                return serializedEvents.GetFrom(fromVersion).Select(DeserializeEvent);

            if (fromVersion != 0)
                throw new IndexOutOfRangeException();

            return new List<IEvent>();
        }

        public void Save(Guid id, IEnumerable<IEvent> events, int persistedVersion)
        {
            AppendOnlyList<string> items = _events.GetOrAdd(id, _ => new AppendOnlyList<string>());

            // FIXME: Unit of work pattern
            foreach (var @event in events)
            {
                items.Append(++persistedVersion, SerializeEvent(@event));
                _eventBus.Publish(@event);
            }
        }

        private static string SerializeEvent(IEvent @event)
        {
            return JsonConvert.SerializeObject(@event, SerializationSettings);
        }
        
        private static IEvent DeserializeEvent(string json)
        {
            return JsonConvert.DeserializeObject<IEvent>(json, SerializationSettings);
        }

        private static JsonSerializerSettings SerializationSettings => new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            };

        private class AppendOnlyList<T> : IEnumerable<T>
        {
            private readonly List<T> _storage = new List<T>();

            public void Append(int version, T entity)
            {
                if (version <= _storage.Count)
                    throw new DBConcurrencyException();

                _storage.Add(entity);
            }

            public IEnumerable<T> GetFrom(int index)
            {
                for (var i = index; i < _storage.Count; i++)
                    yield return _storage[i];
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