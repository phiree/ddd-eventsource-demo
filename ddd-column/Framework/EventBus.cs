using System;
using System.Collections.Generic;
using System.Threading;
using ddd_column.Events;

namespace ddd_column.Framework
{
    public class EventBus : IEventBus
    {
        private readonly List<Action<dynamic>> _observers = new List<Action<dynamic>>();

        public void Publish(IEvent @event)
        {
            ThreadPool.QueueUserWorkItem(state =>
                {
                    foreach (var obs in _observers)
                    {
                        // Poor man's "check if obs implements the right handler
                        try
                        {
                            _observers.Add(ev => obs((dynamic)ev));
                            //obs((dynamic)@event);
                        }
                        catch (Exception) { }
                    }
                });
        }

        public void Subscribe<T>(Action<T> onAction)
            where T : IEvent
        {
        }
    }
}