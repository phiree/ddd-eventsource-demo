using System;
using ddd_column.Domain;

namespace ddd_column.Commands
{
    public class CreateColumn : ICommand
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DataType DataType { get; set; }
    }
}