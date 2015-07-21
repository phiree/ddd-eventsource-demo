using System;
using ddd_column.Commands;

namespace ddd_column.Domain
{
    public class ChangeOperand : ICommand
    {
        public Guid Id { get;  set; }
        public Guid CalculationId { get;  set; }
        public double Operand { get;  set; }
    }
}