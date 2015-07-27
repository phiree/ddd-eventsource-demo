namespace ddd_column.Framework
{
    public interface ISnapshot<out T> : IKeyedObject
        where T : AggregateRoot
    {
        int Version { get; }
        int SchemaVersion { get; }
    }
}