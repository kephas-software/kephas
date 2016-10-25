// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataRepositoryExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data repository extensions class.
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
    /// Extension methods for <see cref="IDataRepository"/>.
    /// </summary>
    public static class DataRepositoryExtensions
    {
        /// <summary>
        /// Searches for the entity with the provided ID and returns it asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="findContext">Context for the find.</param>
        /// <param name="cancellationToken">(Optional) The cancellation token.</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        public static async Task<T> FindAsync<T>(
            this IDataRepository repository,
            IFindContext<T> findContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(repository != null);

            var command = repository.CreateCommand<IFindCommand<T>>();
            var result = await command.ExecuteAsync(findContext, cancellationToken).PreserveThreadContext();
            return result.Entity;
        }

        /// <summary>
        /// Searches for the entity with the provided ID and returns it asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="throwIfNotFound">(Optional) true to throw if not found.</param>
        /// <param name="cancellationToken">(Optional) The cancellation token.</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        public static async Task<T> FindAsync<T>(
            this IDataRepository repository,
            Id id,
            bool throwIfNotFound = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(repository != null);

            var command = repository.CreateCommand<IFindCommand<T>>();
            var findContext = new FindContext<T>(repository, id, throwIfNotFound);
            var result = await command.ExecuteAsync(findContext, cancellationToken).PreserveThreadContext();
            return result.Entity;
        }

        /// <summary>
        /// Searches for the first entity matching the provided criteria and returns it asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="throwIfNotFound">(Optional) true to throw if not found.</param>
        /// <param name="cancellationToken">(Optional) The cancellation token.</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        public static async Task<T> FindOneAsync<T>(
            this IDataRepository repository, 
            Expression<Func<T, bool>> criteria,
            bool throwIfNotFound = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(repository != null);
            Contract.Requires(criteria != null);

            var query = repository.Query<T>().Where(criteria).Take(2);
            var result = await query.ToListAsync(cancellationToken).PreserveThreadContext();
            if (result.Count > 1)
            {
                throw new AmbiguousMatchDataException(string.Format(Strings.DataRepository_FindOneAsync_AmbiguousMatch_Exception, criteria));
            }

            if (result.Count == 0 && throwIfNotFound)
            {
                throw new NotFoundDataException(string.Format(Strings.DataRepository_FindOneAsync_NotFound_Exception, criteria));
            }

            return result.Count == 0 ? default(T) : result[0];
        }

        /// <summary>
        /// Persists the changes in the repository asynchronously.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="cancellationToken">(Optional) The cancellation token.</param>
        /// <returns>
        /// A promise of the persist result.
        /// </returns>
        public static async Task<IDataCommandResult> PersistChangesAsync(
            this IDataRepository repository,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(repository != null);

            var command = repository.CreateCommand<IPersistChangesCommand>();
            var persistContext = new PersistChangesContext(repository);
            var result = await command.ExecuteAsync(persistContext, cancellationToken).PreserveThreadContext();
            return result;
        }
    }
}