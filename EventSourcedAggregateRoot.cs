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

        private readonly List<IEvent> _events = new List<IEvent>();
        public IReadOnlyList<IEvent> UncommittedEvents
        {
            get { return _events; }
        }

        public void Commit(int newVersion)
        {
            if (newVersion != UncommittedEvents.Count + CommittedVersion)
                throw new InvalidOperationException("Should never happen");

            _events.Clear();
            CommittedVersion = newVersion;
        }

        private void ApplyExisting(IEvent @event)
        {
            ((dynamic)this).Apply((dynamic)@event);
            CommittedVersion++;
        }

        protected void ApplyNew(IEvent @event)
        {
            ((dynamic)this).Apply((dynamic)@event);
            _events.Add(@event);
        }
    }
}