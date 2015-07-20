using System;
using System.Collections.Generic;
using System.Linq;

namespace ddd_column
{
    public class Column : EventSourcedAggregateRoot
    {
        private DataType _dataType;
        private bool _isPrimary;

        public Column(Guid id, IEnumerable<IEvent> initialEvents)
            : base(id, initialEvents) { }

        public Column(Guid id, string name, DataType dataType)
            : base(id, Enumerable.Empty<IEvent>())
        {
            ApplyNew(new ColumnCreated(Id, name, dataType));
        }

        public void Apply(ColumnCreated @event)
        {
            _dataType = @event.Type;
        }

        public void Rename(string newName)
        {
            ApplyNew(new ColumnRenamed(Id, newName));
        }

        public void Apply(ColumnRenamed @event)
        {
        }

        public void ChangeDataType(DataType newDataType)
        {
            ApplyNew(new ColumnDataTypeChanged(Id, newDataType));

            if (_isPrimary)
                ApplyNew(new ColumnPrimaryCleared(Id));
        }

        public void Apply(ColumnDataTypeChanged @event)
        {
            _dataType = @event.DataType;
        }

        public void MakePrimary()
        {
            if (_dataType == DataType.Date)
                throw new InvalidOperationException("Dates cannot be primary keys");

            ApplyNew(new ColumnMadePrimary(Id));
        }

        public void Apply(ColumnMadePrimary @event)
        {
            _isPrimary = true;
        }

        public void ClearPrimary()
        {
            ApplyNew(new ColumnPrimaryCleared(Id));
        }

        public void Apply(ColumnPrimaryCleared @event)
        {
            _isPrimary = false;
        }
    }
}