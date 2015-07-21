using System;

namespace ddd_column.Commands
{
    public class ClearColumnPrimary : ICommand
    {
        public Guid Id { get; set; }
    }
}