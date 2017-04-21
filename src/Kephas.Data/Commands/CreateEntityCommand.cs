// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateEntityCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the create entity command base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Behaviors;
    using Kephas.Data.Capabilities;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for create entity commands.
    /// </summary>
    [DataContextType(typeof(DataContextBase))]
    public class CreateEntityCommand : DataCommandBase<ICreateEntityContext, ICreateEntityResult>, ICreateEntityCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateEntityCommand"/> class.
        /// </summary>
        /// <param name="behaviorProvider">The behavior provider.</param>
        protected CreateEntityCommand(IDataBehaviorProvider behaviorProvider)
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
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of a <see cref="ICreateEntityResult" />.
        /// </returns>
        public override async Task<ICreateEntityResult> ExecuteAsync(ICreateEntityContext operationContext, CancellationToken cancellationToken = default(CancellationToken))
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

            var localCache = this.TryGetLocalCache(dataContext);
            if (localCache != null)
            {
                var entityInfo = dataContext.GetEntityInfo(entity);
                localCache.Add(entityInfo.Id, entityInfo);
            }

            // prepare the result
            var result = new CreateEntityResult(entity);
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
        protected virtual object CreateEntity(ICreateEntityContext operationContext)
        {
            // TODO involve an activator, for the case of interfaces.
            var runtimeTypeInfo = operationContext.EntityType.AsRuntimeTypeInfo();
            return runtimeTypeInfo.CreateInstance();
        }

        /// <summary>
        /// Overridable method called just before returning the result.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="result">The result.</param>
        protected virtual void PostCreateEntity(ICreateEntityContext operationContext, ICreateEntityResult result)
        {
        }
    }
}