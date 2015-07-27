using System;
using ddd_column.Events;

namespace ddd_column.Domain
{
    public class Calculation
    {
        public Guid Id { get; }
        private readonly Column _column;

        public double Operand { get; private set; }
        public Operator Operator { get; private set; }

        public static Calculation FromSnapshot(Column column, CalculationSnapshot snapshot)
        {
            return new Calculation(column, snapshot.Id, snapshot.Operand, snapshot.Operator);
        }

        private Calculation(Column column, Guid id, double operand, Operator @operator)
        {
            _column = column;
            Id = id;
            Operand = operand;
            Operator = @operator;
        }

        public Calculation(Column column, Guid id, CalculationAdded @event)
        {
            Id = id;
            _column = column;
            Apply(@event);
        }

        public void Apply(CalculationAdded @event)
        {
            Operand = @event.Operand;
            Operator = @event.Operator;
        }

        public void ChangeOperator(Operator @operator)
        {
            _column.ApplyNew(_column, new CalculationOperatorChanged(_column.Id, Id, @operator));
        }

        public void Apply(CalculationOperatorChanged @event)
        {
            Operator = @event.Operator;
        }

        public void ChangeOperand(double operand)
        {
            _column.ApplyNew(_column, new CalculationOperandChanged(_column.Id, Id, operand));
        }

        public void Apply(CalculationOperandChanged @event)
        {
            Operand = @event.Operand;
        }
    }
}