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
        private Dictionary<Guid, Calculation> _calculations = new Dictionary<Guid, Calculation>();

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

            foreach (Guid calculationId in _calculations.Keys)
                ApplyNew(this, new CalculationRemoved(Id, calculationId));

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

            if (op == Operator.Divide && operand == 0)
                throw new InvalidOperationException("Cannot divide by zero in a calculation");

            ApplyNew(this, new CalculationAdded(Id, calculationId, op, operand));
        }

        public void Apply(CalculationAdded @event)
        {
            _calculations.Add(@event.CalculationId, new Calculation(this, @event.CalculationId, @event));
        }

        public void RemoveCalculation(Guid calculationId)
        {
            ApplyNew(this, new CalculationRemoved(Id, calculationId));
        }

        public void Apply(CalculationRemoved @event)
        {
            _calculations.Remove(@event.CalculationId);
        }

        public void ChangeOperator(Guid calculationId, Operator op)
        {
            Calculation calculation = _calculations[calculationId];

            if (op == Operator.Divide && calculation.Operand == 0)
                throw new InvalidOperationException("Cannot divide by zero in a calculation");

            calculation.ChangeOperator(op);
        }

        public void Apply(CalculationOperatorChanged @event)
        {
            _calculations[@event.CalculationId].Apply(@event);
        }

        public void ChangeOperand(Guid calculationId, double operand)
        {
            Calculation calculation = _calculations[calculationId];
            if (calculation.Operator == Operator.Divide && operand == 0)
                throw new InvalidOperationException("Cannot divide by zero in a calculation");

            calculation.ChangeOperand(operand);
        }

        public void Apply(CalculationOperandChanged @event)
        {
            _calculations[@event.CalculationId].Apply(@event);
        }

        public static ISnapshotter<Column, ColumnSnapshot> Snapshotter
        {
            get { return new ColumnSnapshotter(); }
        }

        private class ColumnSnapshotter : ISnapshotter<Column, ColumnSnapshot>
        {
            public int SchemaVersion { get { return 1; } }

            public ColumnSnapshot TakeSnapshot(Column column)
            {
                return new ColumnSnapshot(column.Id, column.CommittedVersion, SchemaVersion)
                {
                    DataType = column._dataType,
                    IsPrimary = column._isPrimary,
                    Calculations = column._calculations.Values.Select(CalculationSnapshot).ToList()
                };
            }

            private static CalculationSnapshot CalculationSnapshot(Calculation calculation)
            {
                return new CalculationSnapshot
                    {
                        Id = calculation.Id,
                        Operand = calculation.Operand,
                        Operator = calculation.Operator
                    };
            }

            public Column FromSnapshot(ColumnSnapshot snapshot)
            {
                Column column = new Column(snapshot.Id, Enumerable.Empty<IEvent>())
                {
                    _isPrimary = snapshot.IsPrimary,
                    _dataType = snapshot.DataType
                };

                List<Calculation> calculations = snapshot.Calculations.Select(c => Calculation.FromSnapshot(column, c)).ToList();
                column._calculations = calculations.ToDictionary(c => c.Id);

                column.Commit(snapshot.Version);

                return column;
            }
        }
    }
}