using System;
using ddd_column.Commands;

namespace ddd_column.Domain
{
    public class AddCalculation : ICommand
    {
        public Guid Id { get;  set; }
        public Guid CalculationId { get;  set; }
        public Operator Operator { get;  set; }
        public double Operand { get;  set; }
    }
}