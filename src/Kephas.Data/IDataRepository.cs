// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataRepository.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Interface for data repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Reflection;

    /// <summary>
    /// Interface for data repository.
    /// </summary>
    [ContractClass(typeof(DataRepositoryContractClass))]
    public interface IDataRepository : IExpando
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
        Task<T> FindAsync<T>(Id id, IFindContext findContext = null, CancellationToken cancellationToken = default(CancellationToken));

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
        Task<object> FindAsync(ITypeInfo entityType, Id id, IFindContext findContext = null, CancellationToken cancellationToken = default(CancellationToken));

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
        Task<T> FindOneAsync<T>(Expression<Func<T, bool>> criteria, IFindContext findContext = null, CancellationToken cancellationToken = default(CancellationToken));

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
        Task<object> FindOneAsync(ITypeInfo entityType, Expression criteria, IFindContext findContext = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a query over the entity type for the given query context, if any is provided.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="queryContext">Context for the query.</param>
        /// <returns>
        /// A query over the entity type.
        /// </returns>
        IQueryable<T> Query<T>(IQueryContext queryContext = null);

        /// <summary>
        /// Gets a query over the entity type for the given query context, if any is provided.
        /// </summary>
        /// <param name="entityType">  The type of the entity.</param>
        /// <param name="queryContext">Context for the query.</param>
        /// <returns>
        /// A query over the entity type.
        /// </returns>
        IQueryable Query(ITypeInfo entityType, IQueryContext queryContext = null);
    }

    /// <summary>
    /// Contract class for <see cref="IDataRepository"/>.
    /// </summary>
    [ContractClassFor(typeof(IDataRepository))]
    internal abstract class DataRepositoryContractClass : IDataRepository
    {
        /// <summary>
        /// Convenience method that provides a string Indexer
        /// to the Properties collection AND the strongly typed
        /// properties of the object by name.
        /// // dynamic
        /// exp["Address"] = "112 nowhere lane";
        /// // strong
        /// var name = exp["StronglyTypedProperty"] as string;.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object" />.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns>The requested property value.</returns>
        public abstract object this[string key] { get; set; }

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
        public Task<T> FindAsync<T>(Id id, IFindContext findContext = null, CancellationToken cancellationToken = new CancellationToken())
        {
            Contract.Ensures(Contract.Result<Task<T>>() != null);
            return Contract.Result<Task<T>>();
        }

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
        public Task<object> FindAsync(ITypeInfo entityType, Id id, IFindContext findContext = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(entityType != null);
            Contract.Ensures(Contract.Result<Task<object>>() != null);
            return Contract.Result<Task<object>>();
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
        public Task<T> FindOneAsync<T>(
            Expression<Func<T, bool>> criteria,
            IFindContext findContext = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Contract.Requires(criteria != null);
            Contract.Ensures(Contract.Result<Task<T>>() != null);
            return Contract.Result<Task<T>>();
        }

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
        public Task<object> FindOneAsync(ITypeInfo entityType, Expression criteria, IFindContext findContext = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(entityType != null);
            Contract.Requires(criteria != null);
            Contract.Ensures(Contract.Result<Task<object>>() != null);
            return Contract.Result<Task<object>>();
        }

        /// <summary>
        /// Gets a query over the entity type for the given query context, if any is provided.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="queryContext">Context for the query.</param>
        /// <returns>
        /// A query over the entity type.
        /// </returns>
        public IQueryable<T> Query<T>(IQueryContext queryContext = null)
        {
            Contract.Ensures(Contract.Result<IQueryable<T>>() != null);
            return Contract.Result<IQueryable<T>>();
        }

        /// <summary>
        /// Gets a query over the entity type for the given query context, if any is provided.
        /// </summary>
        /// <param name="entityType">  The type of the entity.</param>
        /// <param name="queryContext">Context for the query.</param>
        /// <returns>
        /// A query over the entity type.
        /// </returns>
        public IQueryable Query(ITypeInfo entityType, IQueryContext queryContext = null)
        {
            Contract.Requires(entityType != null);
            Contract.Ensures(Contract.Result<IQueryable>() != null);
            return Contract.Result<IQueryable>();
        }

        /// <summary>
        /// Returns the <see cref="T:System.Dynamic.DynamicMetaObject"/> responsible for binding operations performed on this object.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Dynamic.DynamicMetaObject"/> to bind this object.
        /// </returns>
        /// <param name="parameter">The expression tree representation of the runtime value.</param>
        public abstract DynamicMetaObject GetMetaObject(Expression parameter);
    }
}