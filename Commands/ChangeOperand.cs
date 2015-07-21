using System;
using ddd_column.Commands;

namespace ddd_column.Domain
{
    public class ChangeOperand : ICommand
    {
        public Guid Id { get; private set; }
        public Guid CalculationId { get; private set; }
        public double Operand { get; private set; }
    }
}