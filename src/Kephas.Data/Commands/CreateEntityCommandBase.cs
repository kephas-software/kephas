// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateEntityCommandBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the create entity command base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Data.Capabilities;
using Kephas.Reflection;

namespace Kephas.Data.Commands
{
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Behaviors;
    using Kephas.Diagnostics.Contracts;
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
            Requires.NotNull(behaviorProvider, nameof(behaviorProvider));

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
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">(Optional) the cancellation token.</param>
        /// <returns>
        /// A promise of a <see cref="ICreateEntityResult{TEntity}" />.
        /// </returns>
        public override async Task<ICreateEntityResult<T>> ExecuteAsync(ICreateEntityContext operationContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var entity = this.CreateEntity(operationContext);

            // set the change state to Added
            var dataContext = operationContext.DataContext;
            var trackableEntity = dataContext.TryGetCapability<IChangeStateTrackable>(entity, operationContext);
            if (trackableEntity != null)
            {
                trackableEntity.ChangeState = ChangeState.Added;
            }

            // execute initialization behaviors
            var initializeBehaviors = this.BehaviorProvider.GetDataBehaviors<IOnInitializeBehavior>(entity);
            foreach (var initializeBehavior in initializeBehaviors)
            {
                await initializeBehavior.InitializeAsync(entity, operationContext, cancellationToken).PreserveThreadContext();
            }

            // prepare the result
            var result = new CreateEntityResult<T>(entity);
            this.PostCreateEntity(operationContext, result);

            return result;
        }

        /// <summary>
        /// Creates the entity.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <returns>
        /// The new entity.
        /// </returns>
        protected virtual T CreateEntity(ICreateEntityContext operationContext)
        {
            var runtimeTypeInfo = typeof(T).AsRuntimeTypeInfo();
            return (T) runtimeTypeInfo.CreateInstance();
        }

        /// <summary>
        /// Overridable method called just before returning the result.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="result">The result.</param>
        protected virtual void PostCreateEntity(ICreateEntityContext operationContext, ICreateEntityResult<T> result)
        {
        }
    }
}