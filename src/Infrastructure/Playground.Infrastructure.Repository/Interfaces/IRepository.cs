using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Playground.Infrastructure.Repository.Interfaces
{
    /// <summary>
    /// Repository contract for communicating with a persistance store.
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Expose an <see cref="IQueryable{T}"/> of <typeparamref name="TRecord"/> that allows constructing a linq query against the provided <typeparamref name="TRecord"/>.
        /// </summary>
        /// <typeparam name="TRecord">The type of record to query against.</typeparam>
        /// <param name="query">A <see cref="Func{T, TResult}"/> that provides an initial <see cref="IQueryable{T}"/> of <typeparamref name="TRecord"/> to build a linq query against.
        /// The updated <see cref="IQueryable{T}"/> of <typeparamref name="TRecord"/> should then be returned.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <typeparamref name="TRecord"/> containing any results.</returns>
        IEnumerable<TRecord> ExecuteLinq<TRecord>(Func<IQueryable<TRecord>, IQueryable<TRecord>> query);

        /// <summary>
        /// Tries to create the <paramref name="record"/> on the persistance store.
        /// </summary>
        /// <typeparam name="TRecord">The type of the record to create. It must implement <see cref="IEquatable{T}"/> which should return if two instances are equal to each other.</typeparam>
        /// <param name="record">The record to try and create. It must have an <see cref="int"/> id field.</param>
        /// <param name="idSelector">A selector <see cref="Expression{TDelegate}"/> that returns the <see cref="int"/> field that holds the id for the <typeparamref name="TRecord"/></param>
        /// <returns></returns>
        (bool Successful, int Id) TryCreate<TRecord>(TRecord record, Expression<Func<TRecord, int>> idSelector)
            where TRecord : IEquatable<TRecord>;
    }
}
