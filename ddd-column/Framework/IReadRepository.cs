using System;

namespace ddd_column.Framework
{
    public interface IReadRepository<T>
        where T : IKeyedObject
    {
        T Get(Guid id);
        void Save(T entity);
    }
}