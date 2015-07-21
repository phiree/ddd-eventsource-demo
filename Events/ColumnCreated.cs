using System;
using ddd_column.Domain;

namespace ddd_column.Events
{
    public class ColumnCreated : IEvent
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public DataType Type { get; set; }

        public ColumnCreated(Guid id, string name, DataType type)
        {
            Id = id;
            Name = name;
            Type = type;
        }
    }
}