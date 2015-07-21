using System;
using ddd_column.Commands;

namespace ddd_column.Domain
{
    public class RemoveCalculation : ICommand
    {
        public Guid Id { get; private set; }
        public Guid CalculationId { get; private set; }
    }
}