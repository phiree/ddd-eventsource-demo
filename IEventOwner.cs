namespace ddd_column
{
    public interface IEventOwner<in T> where T : IEvent
    {
        void Apply(T @event);
    }
}