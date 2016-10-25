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

    using Kephas.Composition;
    using Kephas.Data.Commands;
    using Kephas.Data.Commands.Composition;
    using Kephas.Data.Commands.Factory;
    using Kephas.Dynamic;
    using Kephas.Services;

  /// <summary>
    /// Base implementation of a data repository.
    /// </summary>
    public abstract class DataRepositoryBase : Expando, IDataRepository
    {
        /// <summary>
        /// Context for the composition.
        /// </summary>
        private readonly ICompositionContext compositionContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepositoryBase"/> class.
        /// </summary>
        /// <param name="compositionContext">Context for the composition.</param>
        protected DataRepositoryBase(ICompositionContext compositionContext)
        {
            Contract.Requires(compositionContext != null);

            this.compositionContext = compositionContext;
            this.Id = new Id(Guid.NewGuid());
        }

        /// <summary>
        /// Gets the identifier for this instance.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Id Id { get; }

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
        /// Creates the command with the provided type.
        /// </summary>
        /// <typeparam name="TCommand">Type of the command.</typeparam>
        /// <returns>
        /// The new command.
        /// </returns>
        public TCommand CreateCommand<TCommand>()
            where TCommand : IDataCommand
        {
            var commandFactory = this.compositionContext.GetExport<IDataCommandFactory<TCommand>>();
            return commandFactory.CreateCommand(this.GetType());
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
        public virtual TCapability TryGetCapability<TCapability>(object entity, IContext context)
            where TCapability : class
        {
            return entity as TCapability;
        }
    }
}