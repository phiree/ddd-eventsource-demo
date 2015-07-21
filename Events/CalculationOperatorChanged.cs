using System;
using ddd_column.Domain;

namespace ddd_column.Events
{
    public class CalculationOperatorChanged : IEvent
    {
        public CalculationOperatorChanged(Guid id, Guid calculationId, Operator @operator)
        {
            Id = id;
            CalculationId = calculationId;
            Operator = @operator;
        }

        public Guid Id { get; private set; }
        public Guid CalculationId { get; private set; }
        public Operator Operator { get; private set; }
    }
}