namespace ddd_column.Framework
{
    public interface ISnapshotter<T, TSnapshot>
        where TSnapshot : ISnapshot<T>
        where T : AggregateRoot
    {
        int SchemaVersion { get; }
        TSnapshot TakeSnapshot(T column);
        T FromSnapshot(TSnapshot snapshot);
    }
}