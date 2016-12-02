// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateEntityCommandBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the create entity command base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Behaviors;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for create entity commands.
    /// </summary>
    /// <typeparam name="TDataContext">Type of the data context.</typeparam>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public abstract class CreateEntityCommandBase<TDataContext, T> : DataCommandBase<ICreateEntityContext, ICreateEntityResult<T>>, ICreateEntityCommand<TDataContext, T>
        where TDataContext : IDataContext
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateEntityCommandBase{TDataContext,T}"/> class.
        /// </summary>
        /// <param name="behaviorProvider">The behavior provider.</param>
        protected CreateEntityCommandBase(IDataBehaviorProvider behaviorProvider)
        {
            Contract.Requires(behaviorProvider != null);

            this.BehaviorProvider = behaviorProvider;
        }

        /// <summary>
        /// Gets the behavior provider.
        /// </summary>
        /// <value>
        /// The behavior provider.
        /// </value>
        public IDataBehaviorProvider BehaviorProvider { get; }

        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="context">The operationContext.</param>
        /// <param name="cancellationToken">(Optional) the cancellation token.</param>
        /// <returns>
        /// A promise of a <see cref="ICreateEntityResult{TEntity}" />.
        /// </returns>
        public override async Task<ICreateEntityResult<T>> ExecuteAsync(ICreateEntityContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            var entity = this.CreateEntity(context);

            var initializeBehaviors = this.BehaviorProvider.GetDataBehaviors<IOnInitializeBehavior>(entity);
            foreach (var initializeBehavior in initializeBehaviors)
            {
                await initializeBehavior.InitializeAsync(entity, context, cancellationToken).PreserveThreadContext();
            }

            return new CreateEntityResult<T>(entity);
        }

        /// <summary>
        /// Creates the entity.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <returns>
        /// The new entity.
        /// </returns>
        protected abstract T CreateEntity(ICreateEntityContext operationContext);
    }
}