// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data context extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Commands;
    using Kephas.Data.Linq;
    using Kephas.Data.Resources;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Extension methods for <see cref="IDataContext"/>.
    /// </summary>
    public static class DataContextExtensions
    {
        /// <summary>
        /// Searches for the entity with the provided ID and returns it asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="dataContext">The data context.</param>
        /// <param name="findContext">Context for the find operation.</param>
        /// <param name="cancellationToken">(Optional) The cancellation token.</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        public static async Task<T> FindAsync<T>(
            this IDataContext dataContext,
            IFindContext findContext,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            Contract.Requires(dataContext != null);

            var command = dataContext.CreateCommand<IFindCommand<T>>();
            var result = await command.ExecuteAsync(findContext, cancellationToken).PreserveThreadContext();
            return result.Entity;
        }

        /// <summary>
        /// Searches for the entity with the provided ID and returns it asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="dataContext">The data context.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="throwIfNotFound">(Optional) true to throw if not found.</param>
        /// <param name="cancellationToken">(Optional) The cancellation token.</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        public static async Task<T> FindAsync<T>(
            this IDataContext dataContext,
            Id id,
            bool throwIfNotFound = true,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            Contract.Requires(dataContext != null);

            var command = dataContext.CreateCommand<IFindCommand<T>>();
            var findContext = new FindContext<T>(dataContext, id, throwIfNotFound);
            var result = await command.ExecuteAsync(findContext, cancellationToken).PreserveThreadContext();
            return result.Entity;
        }

        /// <summary>
        /// Searches for the first entity matching the provided criteria and returns it asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="dataContext">The data context.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="throwIfNotFound">(Optional) true to throw if not found.</param>
        /// <param name="cancellationToken">(Optional) The cancellation token.</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        public static async Task<T> FindOneAsync<T>(
            this IDataContext dataContext, 
            Expression<Func<T, bool>> criteria,
            bool throwIfNotFound = true,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            Contract.Requires(dataContext != null);
            Contract.Requires(criteria != null);

            var query = dataContext.Query<T>().Where(criteria).Take(2);
            var result = await query.ToListAsync(cancellationToken).PreserveThreadContext();
            if (result.Count > 1)
            {
                throw new AmbiguousMatchDataException(string.Format(Strings.DataContext_FindOneAsync_AmbiguousMatch_Exception, criteria));
            }

            if (result.Count == 0 && throwIfNotFound)
            {
                throw new NotFoundDataException(string.Format(Strings.DataContext_FindOneAsync_NotFound_Exception, criteria));
            }

            return result.Count == 0 ? default(T) : result[0];
        }

        /// <summary>
        /// Persists the changes in the dataContext asynchronously.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="cancellationToken">(Optional) The cancellation token.</param>
        /// <returns>
        /// A promise of the persist result.
        /// </returns>
        public static async Task<IDataCommandResult> PersistChangesAsync(
            this IDataContext dataContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(dataContext != null);

            var command = dataContext.CreateCommand<IPersistChangesCommand>();
            var persistContext = new PersistChangesContext(dataContext);
            var result = await command.ExecuteAsync(persistContext, cancellationToken).PreserveThreadContext();
            return result;
        }
    }
}