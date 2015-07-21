using System;

namespace ddd_column.Commands
{
    public interface ICommand
    {
        Guid Id { get; }
    }
}