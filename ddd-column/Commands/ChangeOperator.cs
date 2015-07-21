using System;
using ddd_column.Commands;

namespace ddd_column.Domain
{
    public class ChangeOperator : ICommand
    {
        public Guid Id { get; private set; }
        public Guid CalculationId { get; private set; }
        public Operator Operator { get; private set; }
    }
}