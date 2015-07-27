using System;

namespace ddd_column.Commands
{
    public class RemoveCalculation : ICommand
    {
        public Guid Id { get;  set; }
        public Guid CalculationId { get;  set; }
    }
}