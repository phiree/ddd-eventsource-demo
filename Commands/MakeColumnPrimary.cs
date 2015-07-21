using System;

namespace ddd_column.Commands
{
    public class MakeColumnPrimary : ICommand
    {
        public Guid Id { get; set; }
    }
}