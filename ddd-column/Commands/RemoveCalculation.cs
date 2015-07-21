using System;
using ddd_column.Commands;

namespace ddd_column.Domain
{
    public class RemoveCalculation : ICommand
    {
        public Guid Id { get;  set; }
        public Guid CalculationId { get;  set; }
    }
}