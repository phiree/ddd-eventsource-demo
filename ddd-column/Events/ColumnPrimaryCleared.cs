using System;

namespace ddd_column.Events
{
    public class ColumnPrimaryCleared : IEvent
    {
        public Guid Id { get; set; }

        public ColumnPrimaryCleared(Guid id)
        {
            Id = id;
        }
    }
}