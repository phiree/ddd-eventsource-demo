using System;
using ddd_column.Domain;

namespace ddd_column.Events
{
    public class CalculationAdded : IEvent
    {
        public CalculationAdded(Guid id, Guid calculationId, Operator @operator, double operand)
        {
            Id = id;
            CalculationId = calculationId;
            Operator = @operator;
            Operand = operand;
        }

        public Guid Id { get; private set; }
        public Guid CalculationId { get; private set; }
        public Operator Operator { get; private set; }
        public double Operand { get; private set; }
    }
}