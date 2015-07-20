using System;

namespace ddd_column
{

    public interface ICommandHandler<in T> where T : ICommand
    {
        void Apply(T command);
    }

    public interface ICommand
    {
        Guid Id { get; }
    }

    public class ClearColumnPrimary : ICommand
    {
        public Guid Id { get; set; }
    }
    public class MakeColumnPrimary : ICommand
    {
        public Guid Id { get; set; }
    }

    public class ChangeColumnDataType : ICommand
    {
        public Guid Id { get; set; }
        public DataType DataType { get; set; }
    }

    public class RenameColumn : ICommand
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class CreateColumn : ICommand
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DataType DataType { get; set; }
    }
}