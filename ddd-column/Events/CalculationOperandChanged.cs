using System;

namespace ddd_column.Events
{
    public class CalculationOperandChanged : IEvent
    {
        public CalculationOperandChanged(Guid id, Guid calculationId, double operand)
        {
            Id = id;
            CalculationId = calculationId;
            Operand = operand;
        }

        public Guid Id { get; private set; }
        public Guid CalculationId { get; private set; }
        public double Operand { get; private set; }
    }
}