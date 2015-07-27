using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ddd_column.Framework
{
    public class MemoryReadRepository<T> : IReadRepository<T>
        where T : IKeyedObject
    {
        private readonly ConcurrentDictionary<Guid, T> _entities = new ConcurrentDictionary<Guid, T>();

        public IEnumerable<T> All
        {
            get { return _entities.Values.ToList(); }
        }

        public T Get(Guid id)
        {
            if (!_entities.ContainsKey(id))
                throw new InvalidOperationException("Not found");

            return _entities[id];
        }

        public void Save(T entity)
        {
            _entities[entity.Id] = entity;
        }

        public void Remove(Guid id)
        {
            T entity;
            _entities.TryRemove(id, out entity);
        }
    }
}