using System;

namespace ddd_column.Framework
{
    public interface IKeyedObject
    {
        Guid Id { get; }
    }
}