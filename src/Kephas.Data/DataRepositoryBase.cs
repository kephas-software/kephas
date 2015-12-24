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
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Reflection;

    /// <summary>
    /// Base implementation of a data repository.
    /// </summary>
    public abstract class DataRepositoryBase : Expando, IDataRepository
    {
        /// <summary>
        /// The <see cref="MethodInfo"/> of the generic <see cref="FindAsync{T}"/> method.
        /// </summary>
        private static MethodInfo findAsyncGenericMethodInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepositoryBase"/> class.
        /// </summary>
        protected DataRepositoryBase()
        {
            EnsureStaticGenericMethodInfosInitialized();
        }

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
        public virtual Task<object> FindAsync(ITypeInfo entityType, Id id, IFindContext findContext = null, CancellationToken cancellationToken = default(CancellationToken))
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
        public virtual Task<object> FindOneAsync(ITypeInfo entityType, Expression criteria, IFindContext findContext = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var entityImplementationType = this.GetEntityImplementationType(entityType);

            var findAsyncMethodInfo = findAsyncGenericMethodInfo.MakeGenericMethod(entityImplementationType);
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
        public virtual IQueryable Query(ITypeInfo entityType, IQueryContext queryContext = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the entity implementation type based on the provided contract type.
        /// </summary>
        /// <param name="contractType">The entity contract type.</param>
        /// <returns>
        /// The entity implementation type.
        /// </returns>
        protected virtual Type GetEntityImplementationType(ITypeInfo contractType)
        {
            Contract.Requires(contractType != null);

            throw new NotImplementedException($"The method '{nameof(this.GetEntityImplementationType)}' must be overridden in derived classes.");
        }

        /// <summary>
        /// Initializes the static generic method infos.
        /// </summary>
        private static void EnsureStaticGenericMethodInfosInitialized()
        {
            if (findAsyncGenericMethodInfo == null)
            {
                var repositoryType = typeof(DataRepositoryBase);
                findAsyncGenericMethodInfo = repositoryType.GetRuntimeMethod(nameof(FindAsync), new[] { typeof(Type), typeof(Id), typeof(IFindContext), typeof(CancellationToken) });
            }
        }
    }
}