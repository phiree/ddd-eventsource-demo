using System;

namespace ddd_column.Commands
{
    public class RenameColumn : ICommand
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}