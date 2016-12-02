// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base implementation of a data dataContext.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Kephas.Composition;
    using Kephas.Data.Commands;
    using Kephas.Data.Commands.Factory;
    using Kephas.Dynamic;
    using Kephas.Services;

  /// <summary>
    /// Base implementation of a <see cref="IDataContext"/>.
    /// </summary>
    public abstract class DataContextBase : Expando, IDataContext
    {
        /// <summary>
        /// Context for the composition.
        /// </summary>
        private readonly ICompositionContext compositionContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextBase"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="compositionContext">Context for the composition.</param>
        protected DataContextBase(IAmbientServices ambientServices, ICompositionContext compositionContext)
        {
            Contract.Requires(ambientServices != null);
            Contract.Requires(compositionContext != null);

            this.AmbientServices = ambientServices;
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
        /// Gets the ambient services.
        /// </summary>
        /// <value>
        /// The ambient services.
        /// </value>
        public IAmbientServices AmbientServices { get; }

        /// <summary>
        /// Gets a query over the entity type for the given query operationContext, if any is provided.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="queryOperationContext">Context for the query.</param>
        /// <returns>
        /// A query over the entity type.
        /// </returns>
        public abstract IQueryable<T> Query<T>(IQueryOperationContext queryOperationContext = null)
            where T : class;

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
        /// <param name="context">The operationContext.</param>
        /// <returns>
        /// The capability.
        /// </returns>
        public virtual TCapability TryGetCapability<TCapability>(object entity, IContext context)
            where TCapability : class
        {
            return entity as TCapability;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c>false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}