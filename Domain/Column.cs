using System;
using System.Collections.Generic;
using System.Linq;
using ddd_column.Events;
using ddd_column.Framework;

namespace ddd_column.Domain
{
    public class Column
        : AggregateRoot
        , IEventSource<ColumnCreated>
        , IEventSource<ColumnRenamed>
        , IEventSource<ColumnPrimaryCleared>
        , IEventSource<ColumnMadePrimary>
        , IEventSource<ColumnDataTypeChanged>
        , IEventSource<CalculationAdded>
        , IEventSource<CalculationRemoved>
        , IEventSource<CalculationOperatorChanged>
        , IEventSource<CalculationOperandChanged>
    {
        private DataType _dataType;
        private bool _isPrimary;

        public Column(Guid id, IEnumerable<IEvent> initialEvents)
            : base(id, initialEvents) { }

        public Column(Guid id, string name, DataType dataType)
            : base(id, Enumerable.Empty<IEvent>())
        {
            ApplyNew(this, new ColumnCreated(Id, name, dataType));
        }

        public void Apply(ColumnCreated @event)
        {
            _dataType = @event.Type;
        }

        public void Rename(string newName)
        {
            ApplyNew(this, new ColumnRenamed(Id, newName));
        }

        public void Apply(ColumnRenamed @event)
        {
        }

        public void ChangeDataType(DataType newDataType)
        {
            if (_isPrimary)
                ApplyNew(this, new ColumnPrimaryCleared(Id));

            foreach (Calculation calculation in _calculations)
                ApplyNew(this, new CalculationRemoved(Id, calculation.Id));

            ApplyNew(this, new ColumnDataTypeChanged(Id, newDataType));
        }

        public void Apply(ColumnDataTypeChanged @event)
        {
            _dataType = @event.DataType;
        }

        public void MakePrimary()
        {
            if (_dataType == DataType.Date)
                throw new InvalidOperationException("Dates cannot be primary keys");

            ApplyNew(this, new ColumnMadePrimary(Id));
        }

        public void Apply(ColumnMadePrimary @event)
        {
            _isPrimary = true;
        }

        public void ClearPrimary()
        {
            ApplyNew(this, new ColumnPrimaryCleared(Id));
        }

        public void Apply(ColumnPrimaryCleared @event)
        {
            _isPrimary = false;
        }

        public void AddCalculation(Guid calculationId, Operator op, double operand)
        {
            if (_dataType != DataType.Number)
                throw new InvalidOperationException("Can only add calculations to numeric columns");

            ApplyNew(this, new CalculationAdded(Id, calculationId, op, operand));
        }

        public void Apply(CalculationAdded @event)
        {
            _calculations.Add(new Calculation(this, @event.CalculationId));
        }

        public void RemoveCalculation(Guid calculationId)
        {
            ApplyNew(this, new CalculationRemoved(Id, calculationId));
        }

        public void Apply(CalculationRemoved @event)
        {
            _calculations.Remove(@event.CalculationId);
        }

        private readonly CalculationCollection _calculations = new CalculationCollection();
        public void ChangeOperator(Guid calculationId, Operator op)
        {
            _calculations[calculationId].ChangeOperator(op);
        }

        public void Apply(CalculationOperatorChanged @event)
        {
            _calculations[@event.CalculationId].Apply(@event);
        }

        public void ChangeOperand(Guid calculationId, double operand)
        {
            _calculations[calculationId].ChangeOperand(operand);
        }

        public void Apply(CalculationOperandChanged @event)
        {
            _calculations[@event.CalculationId].Apply(@event);
        }
    }
}