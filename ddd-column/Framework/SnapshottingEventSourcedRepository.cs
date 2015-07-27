using System;
using System.Collections.Generic;
using ddd_column.Events;

namespace ddd_column.Framework
{
    public interface ISnapshotter<T>
        where T : AggregateRoot
    {
        int SchemaVersion { get; }
        ISnapshot<T> TakeSnapshot(T column);
        T FromSnapshot(ISnapshot<T> snapshot);
    }

    public interface ISnapshot<out T> : IKeyedObject
        where T : AggregateRoot
    {
        int Version { get; }
        int SchemaVersion { get; }
    }

    public class SnapshottingEventSourcedRepository<T> : IEventSourcedRepository<T>
        where T : AggregateRoot
    {
        private readonly Func<Guid, IEnumerable<IEvent>, T> _create;
        private readonly IEventStore _eventStore;
        private readonly IReadRepository<ISnapshot<T>> _snapshotRepository;
        private readonly ISnapshotter<T> _snapshotter;

        public SnapshottingEventSourcedRepository(Func<Guid, IEnumerable<IEvent>, T> create, IEventStore eventStore, IReadRepository<ISnapshot<T>> snapshotRepository, ISnapshotter<T> snapshotter)
        {
            _create = create;
            _eventStore = eventStore;
            _snapshotRepository = snapshotRepository;
            _snapshotter = snapshotter;
        }

        public T Get(Guid id)
        {
            if (!_snapshotRepository.Exists(id))
                return FreshLoad(id);

            ISnapshot<T> snapshot = _snapshotRepository.Get(id);
            if (snapshot.SchemaVersion != _snapshotter.SchemaVersion)
                return FreshLoad(id);

            return LoadFromSnapshot(snapshot);
        }

        private T LoadFromSnapshot(ISnapshot<T> snapshot)
        {
            T aggregateRoot = _snapshotter.FromSnapshot(snapshot);

            IEnumerable<IEvent> eventsAfterSnapshot = _eventStore.EventsFor(snapshot.Id, snapshot.Version);
            foreach (var e in eventsAfterSnapshot)
                aggregateRoot.ApplyExisting(e);

            return aggregateRoot;
        }

        private T FreshLoad(Guid id)
        {
            IEnumerable<IEvent> events = _eventStore.EventsFor(id, 0);
            return _create(id, events);
        }

        public void Save(T aggregateRoot)
        {
            IReadOnlyList<IEvent> events = aggregateRoot.UncommitedEvents;
            _eventStore.Save(aggregateRoot.Id, events, aggregateRoot.CommittedVersion);
            aggregateRoot.Commit(aggregateRoot.CommittedVersion + events.Count);

            if (aggregateRoot.CommittedVersion % 10 == 0)
                _snapshotRepository.Save(_snapshotter.TakeSnapshot(aggregateRoot));
        }
    }
}