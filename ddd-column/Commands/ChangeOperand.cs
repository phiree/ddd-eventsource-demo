using System;

namespace ddd_column.Commands
{
    public class ChangeOperand : ICommand
    {
        public Guid Id { get;  set; }
        public Guid CalculationId { get;  set; }
        public double Operand { get;  set; }
    }
}