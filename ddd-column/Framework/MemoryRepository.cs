using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ddd_column.Events;
using Newtonsoft.Json;

namespace ddd_column.Framework
{
    public class MemoryRepository<T> : IRepository<T>
        where T : IKeyedObject
    {
        private readonly ConcurrentDictionary<Guid, string> _serializedEntities = new ConcurrentDictionary<Guid, string>();

        public IEnumerable<T> All
        {
            get { return _serializedEntities.Values.Select(Deserialize).ToList(); }
        }

        public T Get(Guid id)
        {
            if (!_serializedEntities.ContainsKey(id))
                throw new InvalidOperationException("Not found");

            return Deserialize(_serializedEntities[id]);
        }

        public void Save(T entity)
        {
            _serializedEntities[entity.Id] = Serialize(entity);
        }

        public void Remove(Guid id)
        {
            string json;
            _serializedEntities.TryRemove(id, out json);
        }

        public bool Exists(Guid id)
        {
            return _serializedEntities.ContainsKey(id);
        }

        private static string Serialize(T @event)
        {
            return JsonConvert.SerializeObject(@event, SerializationSettings);
        }

        private static T Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, SerializationSettings);
        }

        private static JsonSerializerSettings SerializationSettings => new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects
        };
    }
}