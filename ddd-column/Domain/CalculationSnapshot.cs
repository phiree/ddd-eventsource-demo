using System;

namespace ddd_column.Domain
{
    public class CalculationSnapshot
    {
        public Guid Id { get; set; }
        public Operator Operator { get; set; }
        public double Operand { get; set; }
    }
}