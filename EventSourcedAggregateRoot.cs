using System;
using System.Collections.Generic;

namespace ddd_column
{
    public abstract class EventSourcedAggregateRoot : AggregateRoot
    {
        public int CommittedVersion { get; private set; }

        protected EventSourcedAggregateRoot(Guid id, IEnumerable<IEvent> initialEvents)
            : base(id)
        {
            foreach (var e in initialEvents)
                ApplyExisting(e);
        }

        private readonly List<IEvent> _uncommitedEvents = new List<IEvent>();
        public IReadOnlyList<IEvent> UncommittedUncommitedEvents
        {
            get { return _uncommitedEvents; }
        }

        public void Commit(int newVersion)
        {
            if (newVersion != UncommittedUncommitedEvents.Count + CommittedVersion)
                throw new InvalidOperationException("Should never happen");

            _uncommitedEvents.Clear();
            CommittedVersion = newVersion;
        }

        private void ApplyExisting(IEvent @event)
        {
            ((dynamic)this).Apply((dynamic)@event);
            CommittedVersion++;
        }

        protected void ApplyNew<TEventOwner, T>(TEventOwner owner, T @event) where TEventOwner : IEventOwner<T> where T : IEvent
        {
            owner.Apply(@event);
            _uncommitedEvents.Add(@event);
        }
    }
}