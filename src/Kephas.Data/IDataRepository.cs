﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataRepository.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Interface for data repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.Linq;
    using System.Linq.Expressions;

    using Kephas.Data.Commands;
    using Kephas.Dynamic;
    using Kephas.Services;

  /// <summary>
    /// Interface for data repository.
    /// </summary>
    [ContractClass(typeof(DataRepositoryContractClass))]
    public interface IDataRepository : IExpando, IIdentifiable
    {
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
        /// Creates the command with the provided type.
        /// </summary>
        /// <typeparam name="TCommand">Type of the command.</typeparam>
        /// <returns>
        /// The new command.
        /// </returns>
        TCommand CreateCommand<TCommand>()
            where TCommand : IDataCommand;

        /// <summary>
        /// Tries to get a capability of the provided entity.
        /// </summary>
        /// <typeparam name="TCapability">Type of the capability.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The capability.
        /// </returns>
        TCapability TryGetCapability<TCapability>(object entity, IContext context)
            where TCapability : class;
    }

  /// <summary>
  /// Contract class for <see cref="IDataRepository"/>.
  /// </summary>
  [ContractClassFor(typeof(IDataRepository))]
    internal abstract class DataRepositoryContractClass : IDataRepository
    {
        /// <summary>
        /// Gets the identifier for this instance.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public abstract Id Id { get; }

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
        /// The <see cref="object" />.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns>The requested property value.</returns>
        public abstract object this[string key] { get; set; }

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
        /// Creates the command with the provided type.
        /// </summary>
        /// <typeparam name="TCommand">Type of the command.</typeparam>
        /// <returns>
        /// The new command.
        /// </returns>
        public TCommand CreateCommand<TCommand>()
            where TCommand : IDataCommand
        {
            Contract.Ensures(Contract.Result<TCommand>() != null);
            return Contract.Result<TCommand>();
        }

        /// <summary>
        /// Tries to get a capability.
        /// </summary>
        /// <typeparam name="TCapability">Type of the capability.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The capability.
        /// </returns>
        public TCapability TryGetCapability<TCapability>(object entity, IContext context)
            where TCapability : class
        {
            return default(TCapability);
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