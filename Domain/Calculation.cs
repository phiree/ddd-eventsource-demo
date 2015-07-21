using System;
using ddd_column.Events;

namespace ddd_column.Domain
{
    public class Calculation
    {
        public Guid Id { get; private set; }
        private readonly Column _column;

        public Calculation(Column column, Guid id)
        {
            Id = id;
            _column = column;
        }

        public void ChangeOperator(Operator @operator)
        {
            _column.ApplyNew(_column, new CalculationOperatorChanged(_column.Id, Id, @operator));
        }

        public void Apply(CalculationOperatorChanged @event)
        {
        }

        public void ChangeOperand(double operand)
        {
            _column.ApplyNew(_column, new CalculationOperandChanged(_column.Id, Id, operand));
        }

        public void Apply(CalculationOperandChanged @event)
        {
        }
    }
}