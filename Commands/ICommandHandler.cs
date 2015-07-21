namespace ddd_column.Commands
{
    public interface ICommandHandler<in T> where T : ICommand
    {
        void Apply(T command);
    }
}