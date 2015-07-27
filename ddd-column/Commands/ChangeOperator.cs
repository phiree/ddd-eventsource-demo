using System;
using ddd_column.Domain;

namespace ddd_column.Commands
{
    public class ChangeOperator : ICommand
    {
        public Guid Id { get;  set; }
        public Guid CalculationId { get;  set; }
        public Operator Operator { get;  set; }
    }
}