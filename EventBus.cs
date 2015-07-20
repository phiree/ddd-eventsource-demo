using System;
using System.Collections.Generic;

namespace ddd_column
{
    public interface IEventBus
    {
        void Publish(IEvent @event);
    }

    public class EventBus : IEventBus
    {
        private List<Action<dynamic>> _observers = new List<Action<dynamic>>();

        public void Publish(IEvent @event)
        {
            foreach (var obs in _observers)
            {
                // Poor man's "check if obs implements the right handler
                try
                {
                    obs((dynamic)@event);
                }
                catch (Exception) { }
            }
        }

        public void Subscribe<T>(Action<T> onAction)
            where T : IEvent
        {
            _observers.Add(ev => onAction(ev));
        }
    }

    public class UnitOfWorkEventBus : IEventBus
    {
        private readonly IEventBus _realBus;
        private readonly IList<IEvent> _events = new List<IEvent>();

        public UnitOfWorkEventBus(IEventBus realBus)
        {
            _realBus = realBus;
        }

        public void Publish(IEvent @event)
        {
            _events.Add(@event);
        }

        public void Commit()
        {
            foreach (var e in _events)
                _realBus.Publish(e);
        }

        public void Revert()
        {
            _events.Clear();
        }
    }
}