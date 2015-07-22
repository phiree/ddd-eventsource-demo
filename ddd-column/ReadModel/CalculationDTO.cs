using System;
using ddd_column.Domain;
using ddd_column.Framework;

namespace ddd_column.ReadModel
{
    public class CalculationDTO : IKeyedObject
    {
        public Guid Id { get; private set; }

        public CalculationDTO(Guid id)
        {
            Id = id;
        }

        public Guid ColumnId { get; set; }
        public Operator Operator { get; set; }
        public double Operand { get; set; }
    }
}