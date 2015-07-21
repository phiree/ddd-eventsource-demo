using System;
using System.Collections.Generic;
using ddd_column.Events;

namespace ddd_column.Framework
{
    public abstract class AggregateRoot
    {
        public Guid Id { get; private set; }
        public int CommittedVersion { get; private set; }

        protected AggregateRoot(Guid id, IEnumerable<IEvent> initialEvents)
        {
            Id = id;

            foreach (var e in initialEvents)
                ApplyExisting(e);
        }

        private readonly List<IEvent> _uncommitedEvents = new List<IEvent>();
        public IReadOnlyList<IEvent> UncommitedEvents
        {
            get { return _uncommitedEvents; }
        }

        public void Commit(int newVersion)
        {
            if (newVersion != UncommitedEvents.Count + CommittedVersion)
                throw new InvalidOperationException("Should never happen");

            _uncommitedEvents.Clear();
            CommittedVersion = newVersion;
        }

        private void ApplyExisting(IEvent @event)
        {
            ((dynamic)this).Apply((dynamic)@event);
            CommittedVersion++;
        }

        protected internal void ApplyNew<TEventOwner, T>(TEventOwner owner, T @event)
            where TEventOwner : AggregateRoot, IEventSource<T>
            where T : IEvent
        {
            if (owner != this)
                throw new InvalidOperationException("Can only apply event on self");

            owner.Apply(@event);
            _uncommitedEvents.Add(@event);
        }
    }
}