using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;

namespace ddd_column.Domain
{
    public sealed class ImmutableKeyedCollection<TKey, T> : IEnumerable<T>
    {
        private readonly ImmutableList<T> _entities;
        private readonly ImmutableSortedDictionary<TKey, T> _entityLookup;
        private readonly Func<T, TKey> _getKey;

        public ImmutableKeyedCollection(Func<T, TKey> getKey)
            : this(ImmutableList.Create<T>(), ImmutableSortedDictionary.Create<TKey, T>(), getKey)
        {
        }

        private ImmutableKeyedCollection(ImmutableList<T> entities, ImmutableSortedDictionary<TKey, T> entityLookup, Func<T, TKey> getKey)
        {
            _entities = entities;
            _entityLookup = entityLookup;
            _getKey = getKey;
        }

        [Pure]
        public IEnumerator<T> GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        [Pure]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [Pure]
        public ImmutableKeyedCollection<TKey, T> Add(T item)
        {
            TKey key = _getKey(item);
            ImmutableList<T> entities = _entities.Add(item);
            ImmutableSortedDictionary<TKey, T> lookup = _entityLookup.Add(key, item);

            return new ImmutableKeyedCollection<TKey, T>(entities, lookup, _getKey);
        }

        [Pure]
        public ImmutableKeyedCollection<TKey, T> Remove(TKey key)
        {
            ImmutableList<T> entities = _entities.Remove(_entityLookup[key]);
            ImmutableSortedDictionary<TKey, T> lookup = _entityLookup.Remove(key);

            return new ImmutableKeyedCollection<TKey, T>(entities, lookup, _getKey);
        }

        public T this[TKey index]
        {
            get { return _entityLookup[index]; }
        }
    }

}