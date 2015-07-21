using System;
using System.Collections.Generic;
using ddd_column.Events;

namespace ddd_column.Framework
{
    public class EventBus : IEventBus
    {
        private readonly List<Action<dynamic>> _observers = new List<Action<dynamic>>();

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
}