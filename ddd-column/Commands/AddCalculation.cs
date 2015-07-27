using System;
using ddd_column.Domain;

namespace ddd_column.Commands
{
    public class AddCalculation : ICommand
    {
        public Guid Id { get;  set; }
        public Guid CalculationId { get;  set; }
        public Operator Operator { get;  set; }
        public double Operand { get;  set; }
    }
}