using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Playground.Infrastructure.Repository.Interfaces;

namespace Playground.Infrastructure.Repository.InMemory
{
    /// <summary>
    /// A simple in memory implementation of an <see cref="IRepository"/> using <see cref="ConcurrentDictionary{TKey, TValue}"/> as the data store.
    /// </summary>
    public class InMemoryDictionaryRepository : IRepository
    {
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<int, object>> _data
            = new ConcurrentDictionary<Type, ConcurrentDictionary<int, object>>();
        private readonly object _idLock = new object();
        private int _nextId = 1;

        /// <inheritdoc/>
        public IEnumerable<TRecord> ExecuteLinq<TRecord>(Func<IQueryable<TRecord>, IQueryable<TRecord>> query)
        {
            if (query is null)
                throw new ArgumentNullException(nameof(query));

            if (!_data.TryGetValue(typeof(TRecord), out ConcurrentDictionary<int, object>? table))
                return Enumerable.Empty<TRecord>();

            return query(table.Values.Cast<TRecord>().AsQueryable()).AsEnumerable();
        }

        /// <inheritdoc/>
        public (bool Successful, int Id) TryCreate<TRecord>(TRecord record, Expression<Func<TRecord, int>> idSelector)
            where TRecord : IEquatable<TRecord>
        {
            if (idSelector is null)
                throw new ArgumentNullException(nameof(idSelector));

            ConcurrentDictionary<int, object> table = _data.GetOrAdd(typeof(TRecord), _ => new ConcurrentDictionary<int, object>());

            int id;
            lock (_idLock)
            {
                id = _nextId++;
            }
            BinaryExpression idAssignment = Expression.Assign(idSelector.Body, Expression.Constant(id));
            var assign = Expression.Lambda<Action<TRecord>>(
                idAssignment,
                idSelector.Parameters.Single());
            assign.Compile().Invoke(record);

            bool success = table.TryAdd(id, record);
            return (success, id);
        }
    }
}
