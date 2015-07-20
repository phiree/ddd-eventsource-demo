using System;

namespace ddd_column
{
    public class Column : AggregateRoot
    {
        private string _name;
        private DataType _dataType;
        private bool _isPrimary;
        private readonly IEventBus _bus;

        public Column(IEventBus bus, Guid id, string name, DataType dataType)
            : base(id)
        {
            _bus = bus;
            _name = name;
            _dataType = dataType;
            bus.Publish(new ColumnCreated(Id, _name, _dataType));
        }

        public void Rename(string newName)
        {
            _name = newName;

            _bus.Publish(new ColumnRenamed(Id, newName));
        }

        public void ChangeDataType(DataType newDataType)
        {
            _dataType = newDataType;
            _isPrimary = false;

            _bus.Publish(new ColumnDataTypeChanged(Id, newDataType));
        }

        public void MakePrimary()
        {
            if (_dataType == DataType.Date)
                throw new InvalidOperationException("Dates cannot be primary keys");

            _isPrimary = true;

            _bus.Publish(new ColumnMadePrimary(Id));
        }

        public void ClearPrimary()
        {
            _isPrimary = false;
            _bus.Publish(new ColumnPrimaryCleared(Id));
        }
    }
}