using System;
using System.Collections.Generic;
using ddd_column.Events;

namespace ddd_column.Framework
{
    public interface IEventStore
    {
        IEnumerable<IEvent> EventsFor(Guid id, int fromVersion);

        void Save(Guid id, IEnumerable<IEvent> @events, int persistedVersion);
    }
}