using System;
using System.Collections.Generic;
using ddd_column.Events;

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

    public interface ISnapshot<out T> : IKeyedObject
        where T : AggregateRoot
    {
        int Version { get; }
        int SchemaVersion { get; }
    }

    public class SnapshottingEventSourcedRepository<T, TSnapshot> : IEventSourcedRepository<T>
        where TSnapshot : ISnapshot<T>
        where T : AggregateRoot
    {
        private readonly Func<Guid, IEnumerable<IEvent>, T> _create;
        private readonly IEventStore _eventStore;
        private readonly IReadRepository<TSnapshot> _snapshotRepository;
        private readonly ISnapshotter<T, TSnapshot> _snapshotter;
        private readonly int _eventsPerSnapshot;

        public SnapshottingEventSourcedRepository(Func<Guid, IEnumerable<IEvent>, T> create, IEventStore eventStore, IReadRepository<TSnapshot> snapshotRepository, ISnapshotter<T, TSnapshot> snapshotter, int eventsPerSnapshot)
        {
            _create = create;
            _eventStore = eventStore;
            _snapshotRepository = snapshotRepository;
            _snapshotter = snapshotter;
            _eventsPerSnapshot = eventsPerSnapshot;
        }

        public T Get(Guid id)
        {
            if (!_snapshotRepository.Exists(id))
                return FreshLoad(id);

            TSnapshot snapshot = _snapshotRepository.Get(id);
            if (snapshot.SchemaVersion != _snapshotter.SchemaVersion)
                return FreshLoad(id);

            return LoadFromSnapshot(snapshot);
        }

        private T LoadFromSnapshot(TSnapshot snapshot)
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

            if (aggregateRoot.CommittedVersion % _eventsPerSnapshot == 0)
                _snapshotRepository.Save(_snapshotter.TakeSnapshot(aggregateRoot));
        }
    }
}