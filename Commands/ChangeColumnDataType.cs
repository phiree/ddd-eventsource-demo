using System;
using ddd_column.Domain;

namespace ddd_column.Commands
{
    public class ChangeColumnDataType : ICommand
    {
        public Guid Id { get; set; }
        public DataType DataType { get; set; }
    }
}