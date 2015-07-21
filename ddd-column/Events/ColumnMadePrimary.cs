using System;

namespace ddd_column.Events
{
    public class ColumnMadePrimary : IEvent
    {
        public Guid Id { get; set; }

        public ColumnMadePrimary(Guid id)
        {
            Id = id;
        }
    }
}