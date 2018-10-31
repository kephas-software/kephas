// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryableHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the queryable helper class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Resources;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Extension methods for <see cref="IQueryable{T}"/>.
    /// </summary>
    public static class QueryableHelper
    {
        /// <summary>
        /// Returns a long that represents the number of elements in a sequence.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A promise of a long that represents the number of elements in a sequence.</returns>
        public static Task<long> LongCountAsync<T>(this IQueryable<T> query, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(query, nameof(query));

            if (query.Provider is IAsyncQueryProvider asyncProvider)
            {
                var longCountExpression = Expression.Call(
                    null,
                    GetMethodInfo(Queryable.LongCount, query),
                    query.Expression);

                return asyncProvider.ExecuteAsync<long>(longCountExpression, cancellationToken);
            }

            return Task.FromResult(query.LongCount());
        }

        /// <summary>
        /// Returns a long that represents the number of elements in a sequence.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A promise of a long that represents the number of elements in a sequence.</returns>
        public static Task<long> LongCountAsync(this IQueryable query, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(query, nameof(query));

            return LongCountAsync<object>((IQueryable<object>)query, cancellationToken);
        }

        /// <summary>
        /// Returns the number of elements in a sequence.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A promise of the number of elements in a sequence.</returns>
        public static Task<int> CountAsync<T>(this IQueryable<T> query, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(query, nameof(query));

            var asyncProvider = query.Provider as IAsyncQueryProvider;
            if (asyncProvider != null)
            {
                var countExpression = Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Count, query),
                    query.Expression);

                return asyncProvider.ExecuteAsync<int>(countExpression, cancellationToken);
            }

            return Task.FromResult(query.Count());
        }

        /// <summary>
        /// Returns the number of elements in a sequence.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A promise of the number of elements in a sequence.</returns>
        public static Task<int> CountAsync(this IQueryable query, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(query, nameof(query));

            return CountAsync<object>((IQueryable<object>)query, cancellationToken);
        }

        /// <summary>
        /// Returns the single element of a sequence that satisfies a specified condition, or a default value, if no such element is found.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="predicate">The condition to be satisfied.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A promise of the single element of a sequence.</returns>
        public static Task<T> SingleOrDefaultAsync<T>(this IQueryable<T> query, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(query, nameof(query));
            Requires.NotNull(predicate, nameof(predicate));

            var asyncProvider = query.Provider as IAsyncQueryProvider;
            if (asyncProvider != null)
            {
                var singleExpression = (Expression)Expression.Call(
                    null,
                    GetMethodInfo(Queryable.SingleOrDefault, query, predicate),
                    new[] { query.Expression, Expression.Quote(predicate) });

                return asyncProvider.ExecuteAsync<T>(singleExpression, cancellationToken);
            }

            return Task.FromResult(query.SingleOrDefault(predicate));
        }

        /// <summary>
        /// Returns the single element of a sequence, or a default value, if the sequence contains no elements.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A promise of the single element of a sequence.</returns>
        public static Task<T> SingleOrDefaultAsync<T>(this IQueryable<T> query, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(query, nameof(query));

            var asyncProvider = query.Provider as IAsyncQueryProvider;
            if (asyncProvider != null)
            {
                var singleExpression = Expression.Call(
                    null,
                    GetMethodInfo(Queryable.SingleOrDefault, query),
                    query.Expression);

                return asyncProvider.ExecuteAsync<T>(singleExpression, cancellationToken);
            }

            return Task.FromResult(query.SingleOrDefault());
        }

        /// <summary>
        /// Returns the single element of a sequence, or a default value, if the sequence contains no elements.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A promise of the single element of a sequence.</returns>
        public static Task<object> SingleOrDefaultAsync(this IQueryable query, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(query, nameof(query));

            return SingleOrDefaultAsync<object>((IQueryable<object>)query, cancellationToken);
        }

        /// <summary>
        /// Returns the single element of a sequence that satisfies a specified condition.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="predicate">The condition to be satisfied.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A promise of the single element of a sequence.</returns>
        public static Task<T> SingleAsync<T>(this IQueryable<T> query, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(query, nameof(query));
            Requires.NotNull(predicate, nameof(predicate));

            var asyncProvider = query.Provider as IAsyncQueryProvider;
            if (asyncProvider != null)
            {
                var singleExpression = (Expression)Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Single, query, predicate),
                    new[] { query.Expression, Expression.Quote(predicate) });

                return asyncProvider.ExecuteAsync<T>(singleExpression, cancellationToken);
            }

            return Task.FromResult(query.Single(predicate));
        }

        /// <summary>
        /// Returns the single element of a sequence.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A promise of the single element of a sequence.</returns>
        public static Task<T> SingleAsync<T>(this IQueryable<T> query, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(query, nameof(query));

            var asyncProvider = query.Provider as IAsyncQueryProvider;
            if (asyncProvider != null)
            {
                var singleExpression = Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Single, query),
                    query.Expression);

                return asyncProvider.ExecuteAsync<T>(singleExpression, cancellationToken);
            }

            return Task.FromResult(query.Single());
        }

        /// <summary>
        /// Returns the single element of a sequence.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A promise of the single element of a sequence.</returns>
        public static Task<object> SingleAsync(this IQueryable query, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(query, nameof(query));

            return SingleAsync<object>((IQueryable<object>)query, cancellationToken);
        }

        /// <summary>
        /// Returns the first element of a sequence that satisfies a specified condition, or a default value, if no such element is found.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="predicate">The condition to be satisfied.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A promise of the first element of a sequence.</returns>
        public static Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> query, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(query, nameof(query));
            Requires.NotNull(predicate, nameof(predicate));

            var asyncProvider = query.Provider as IAsyncQueryProvider;
            if (asyncProvider != null)
            {
                var firstExpression = (Expression)Expression.Call(
                    null,
                    GetMethodInfo(Queryable.FirstOrDefault, query, predicate),
                    new[] { query.Expression, Expression.Quote(predicate) });

                return asyncProvider.ExecuteAsync<T>(firstExpression, cancellationToken);
            }

            return Task.FromResult(query.FirstOrDefault(predicate));
        }

        /// <summary>
        /// Returns the first element of a sequence, or a default value, if the sequence contains no elements.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A promise of the first element of a sequence.</returns>
        public static Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> query, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(query, nameof(query));

            var asyncProvider = query.Provider as IAsyncQueryProvider;
            if (asyncProvider != null)
            {
                var firstExpression = Expression.Call(
                    null,
                    GetMethodInfo(Queryable.FirstOrDefault, query),
                    query.Expression);

                return asyncProvider.ExecuteAsync<T>(firstExpression, cancellationToken);
            }

            return Task.FromResult(query.FirstOrDefault());
        }

        /// <summary>
        /// Returns the first element of a sequence, or a default value, if the sequence contains no elements.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A promise of the first element of a sequence.</returns>
        public static Task<object> FirstOrDefaultAsync(this IQueryable query, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(query, nameof(query));

            return FirstOrDefaultAsync<object>((IQueryable<object>)query, cancellationToken);
        }

        /// <summary>
        /// Returns the first element of a sequence that satisfies a specified condition.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="predicate">The condition to be satisfied.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A promise of the first element of a sequence.</returns>
        public static Task<T> FirstAsync<T>(this IQueryable<T> query, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(query, nameof(query));
            Requires.NotNull(predicate, nameof(predicate));

            var asyncProvider = query.Provider as IAsyncQueryProvider;
            if (asyncProvider != null)
            {
                var firstExpression = (Expression)Expression.Call(
                    null,
                    GetMethodInfo(Queryable.First, query, predicate),
                    new[] { query.Expression, Expression.Quote(predicate) });

                return asyncProvider.ExecuteAsync<T>(firstExpression, cancellationToken);
            }

            return Task.FromResult(query.First(predicate));
        }

        /// <summary>
        /// Returns the first element of a sequence.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A promise of the first element of a sequence.</returns>
        public static Task<T> FirstAsync<T>(this IQueryable<T> query, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(query, nameof(query));

            var asyncProvider = query.Provider as IAsyncQueryProvider;
            if (asyncProvider != null)
            {
                var firstExpression = Expression.Call(
                    null,
                    GetMethodInfo(Queryable.First, query),
                    query.Expression);

                return asyncProvider.ExecuteAsync<T>(firstExpression, cancellationToken);
            }

            return Task.FromResult(query.First());
        }

        /// <summary>
        /// Returns the first element of a sequence.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A promise of the first element of a sequence.</returns>
        public static Task<object> FirstAsync(this IQueryable query, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(query, nameof(query));

            return FirstAsync<object>((IQueryable<object>)query, cancellationToken);
        }

        /// <summary>
        /// Determines asynchronously whether a sequence contains any elements.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A promise of a boolean value indicating whether a sequence contains any elements.</returns>
        public static Task<bool> AnyAsync<T>(this IQueryable<T> query, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(query, nameof(query));

            var asyncProvider = query.Provider as IAsyncQueryProvider;
            if (asyncProvider != null)
            {
                var anyExpression = Expression.Call(
                    null,
                    GetMethodInfo(Queryable.Any, query),
                    query.Expression);

                return asyncProvider.ExecuteAsync<bool>(anyExpression, cancellationToken);
            }

            return Task.FromResult(query.Any());
        }

        /// <summary>
        /// Determines asynchronously whether a sequence contains any elements.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A promise of a boolean value indicating whether a sequence contains any elements.</returns>
        public static Task<bool> AnyAsync(
            this IQueryable query,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(query, nameof(query));

            return AnyAsync<object>((IQueryable<object>)query, cancellationToken);
        }

        /// <summary>
        /// Executes the query asynchronously and returns a list of items.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of items.</returns>
        public static async Task<IList<T>> ToListAsync<T>(this IQueryable<T> query, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(query, nameof(query));

            if (query.Provider is IAsyncQueryProvider asyncProvider)
            {
                var result = await asyncProvider.ExecuteAsync(query.Expression, cancellationToken).PreserveThreadContext();

                if (result == null)
                {
                    return null;
                }

                if (result is IList<T> listResult)
                {
                    return listResult;
                }

                if (result is IEnumerable<T> enumerableResult)
                {
                    return enumerableResult.ToList();
                }

                if (result is IEnumerable<object> objectEnumerableResult)
                {
                    return objectEnumerableResult.Cast<T>().ToList();
                }

                if (result is T objectResult)
                {
                    return new List<T> { objectResult };
                }

                throw new DataException(string.Format(Strings.QueryHelper_ToListAsync_CannotConvertToListException, result, result.GetType(), typeof(T)));
            }

            return query.ToList();
        }

        /// <summary>
        /// Executes the query asynchronously and returns a list of items.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of items.</returns>
        public static Task<IList<object>> ToListAsync(this IQueryable query, CancellationToken cancellationToken = default)
        {
            return ToListAsync<object>((IQueryable<object>)query, cancellationToken);
        }

        /// <summary>
        /// Gets the method information for the provided function definition.
        /// </summary>
        /// <typeparam name="T1">Generic type parameter T1.</typeparam>
        /// <typeparam name="T2">Generic type parameter T2.</typeparam>
        /// <param name="f">A Func{T1,T2} to process.</param>
        /// <param name="unused1">The parameter is not used.</param>
        /// <returns>
        /// The method information.
        /// </returns>
        private static MethodInfo GetMethodInfo<T1, T2>(Func<T1, T2> f, T1 unused1)
        {
            return f.GetMethodInfo();
        }

        /// <summary>
        /// Gets the method information for the provided function definition.
        /// </summary>
        /// <typeparam name="T1">Generic type parameter T1.</typeparam>
        /// <typeparam name="T2">Generic type parameter T2.</typeparam>
        /// <typeparam name="T3">Generic type parameter T3.</typeparam>
        /// <param name="f">A Func{T1,T2,T3} to process.</param>
        /// <param name="unused1">The parameter is not used.</param>
        /// <param name="unused2">The parameter is not used.</param>
        /// <returns>
        /// The method information.
        /// </returns>
        private static MethodInfo GetMethodInfo<T1, T2, T3>(Func<T1, T2, T3> f, T1 unused1, T2 unused2)
        {
            return f.GetMethodInfo();
        }
    }
}