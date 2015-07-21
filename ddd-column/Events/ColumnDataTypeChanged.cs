using System;
using ddd_column.Domain;

namespace ddd_column.Events
{
    public class ColumnDataTypeChanged : IEvent
    {
        public Guid Id { get; set; }
        public DataType DataType { get; set; }

        public ColumnDataTypeChanged(Guid id, DataType dataType)
        {
            Id = id;
            DataType = dataType;
        }
    }
}