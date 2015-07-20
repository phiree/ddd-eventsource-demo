using System;

namespace ddd_column
{
    public abstract class AggregateRoot
    {
        protected AggregateRoot(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; private set; }
    }
}