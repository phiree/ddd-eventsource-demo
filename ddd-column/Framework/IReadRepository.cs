using System;
using System.Collections.Generic;

namespace ddd_column.Framework
{
    public interface IReadRepository<T>
        where T : IKeyedObject
    {
        IEnumerable<T> All { get; }
        T Get(Guid id);
        void Save(T entity);
        void Remove(Guid id);

        bool Exists(Guid id);
    }
}