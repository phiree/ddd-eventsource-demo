﻿using System;
using System.Collections.Generic;

namespace ddd_column
{
    public interface IRepository<T>
        where T : AggregateRoot
    {
        T Get(Guid id);
        void Save(T aggregateRoot);
    }

    public class MemoryRepository<T> : IRepository<T>
           where T : AggregateRoot
    {
        private readonly Dictionary<Guid, T> _entities = new Dictionary<Guid, T>();

        public T Get(Guid id)
        {
            if (!_entities.ContainsKey(id))
                throw new InvalidOperationException("Not found");

            return _entities[id];
        }

        public void Save(T aggregateRoot)
        {
            _entities[aggregateRoot.Id] = aggregateRoot;
        }
    }

    public class UnitOfWorkRepository<T> : IRepository<T> where T : AggregateRoot
    {
        private readonly IRepository<T> _repository;
        private readonly UnitOfWorkEventBus _bus;

        public UnitOfWorkRepository(IRepository<T> repository, UnitOfWorkEventBus bus)
        {
            _repository = repository;
            _bus = bus;
        }

        public T Get(Guid id)
        {
            T entity = _repository.Get(id);
            _bus.Revert(); // HACK

            return entity;
        }

        public void Save(T aggregateRoot)
        {
            try
            {
                _repository.Save(aggregateRoot);
                _bus.Commit();
            }
            catch (Exception)
            {
                _bus.Revert();
            }
        }
    }
}