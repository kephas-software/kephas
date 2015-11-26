// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataRepositoryBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base implementation of a data repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;

    /// <summary>
    /// Base implementation of a data repository.
    /// </summary>
    public abstract class DataRepositoryBase : Expando, IDataRepository
    {
        /// <summary>
        /// Searches for the entity with the provided ID and returns it asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="id">               The identifier.</param>
        /// <param name="findContext">      Context for the find.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        public abstract Task<T> FindAsync<T>(
            Id id,
            IFindContext findContext = null,
            CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Searches for the entity with the provided ID and returns it asynchronously.
        /// </summary>
        /// <param name="entityType">       The type of the entity.</param>
        /// <param name="id">               The identifier.</param>
        /// <param name="findContext">      Context for the find.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        public virtual Task<object> FindAsync(
            Type entityType,
            Id id,
            IFindContext findContext = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches for the first entity matching the provided criteria and returns it asynchronously.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="criteria">         The criteria.</param>
        /// <param name="findContext">      Context for the find.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        public abstract Task<T> FindOneAsync<T>(
            Expression<Func<T, bool>> criteria,
            IFindContext findContext = null,
            CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Searches for the first entity matching the provided criteria and returns it asynchronously.
        /// </summary>
        /// <param name="entityType">       The type of the entity.</param>
        /// <param name="criteria">         The criteria.</param>
        /// <param name="findContext">      Context for the find.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        public virtual Task<object> FindOneAsync(
            Type entityType,
            Expression criteria,
            IFindContext findContext = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a query over the entity type for the given query context, if any is provided.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="queryContext">Context for the query.</param>
        /// <returns>
        /// A query over the entity type.
        /// </returns>
        public abstract IQueryable<T> Query<T>(IQueryContext queryContext = null);

        /// <summary>
        /// Gets a query over the entity type for the given query context, if any is provided.
        /// </summary>
        /// <param name="entityType">  The type of the entity.</param>
        /// <param name="queryContext">Context for the query.</param>
        /// <returns>
        /// A query over the entity type.
        /// </returns>
        public virtual IQueryable Query(Type entityType, IQueryContext queryContext = null)
        {
            throw new NotImplementedException();
        }
    }
}