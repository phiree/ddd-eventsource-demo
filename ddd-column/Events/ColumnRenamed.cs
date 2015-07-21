using System;

namespace ddd_column.Events
{
    public class ColumnRenamed : IEvent
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }

        public ColumnRenamed(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}