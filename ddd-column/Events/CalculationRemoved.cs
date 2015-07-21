using System;

namespace ddd_column.Events
{
    public class CalculationRemoved : IEvent
    {
        public CalculationRemoved(Guid id, Guid calculationId)
        {
            Id = id;
            CalculationId = calculationId;
        }

        public Guid Id { get; private set; }
        public Guid CalculationId { get; private set; }
    }
}