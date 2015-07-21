using System;
using System.Collections.Generic;

namespace ddd_column.Framework
{
    public class MemoryReadRepository<T> : IReadRepository<T>
        where T : IKeyedObject
    {
        private readonly Dictionary<Guid, T> _entities = new Dictionary<Guid, T>();

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
    }
}