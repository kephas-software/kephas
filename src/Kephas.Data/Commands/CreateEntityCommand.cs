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
    using Kephas.Runtime;
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
        public CreateEntityCommand(IDataBehaviorProvider behaviorProvider)
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
        public override async Task<ICreateEntityResult> ExecuteAsync(ICreateEntityContext operationContext, CancellationToken cancellationToken = default)
        {
            var entity = this.CreateEntity(operationContext);

            var dataContext = operationContext.DataContext;
            IEntityInfo entityInfo = null;
            try
            {
                entityInfo = dataContext.AttachEntity(entity);

                // set the change state to Added
                entityInfo.ChangeState = ChangeState.Added;

                // execute initialization behaviors
                var initializeBehaviors = this.BehaviorProvider.GetDataBehaviors<IOnInitializeBehavior>(entity);
                foreach (var initializeBehavior in initializeBehaviors)
                {
                    await initializeBehavior.InitializeAsync(entity, entityInfo, operationContext, cancellationToken).PreserveThreadContext();
                }

                // prepare the result
                var result = new CreateEntityResult(entity, entityInfo);
                this.PostCreateEntity(operationContext, result);

                // all the changes in the initialization should be reset.
                // only after this point, the entity may be consider fully initialized.
                entityInfo.AcceptChanges();
                entityInfo.ChangeState = ChangeState.Added;

                return result;
            }
            catch
            {
                if (entityInfo != null)
                {
                    dataContext.DetachEntity(entityInfo);
                }

                throw;
            }
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
            var activator = this.TryGetEntityActivator(operationContext.DataContext) ?? RuntimeActivator.Instance;
            var entity = activator.CreateInstance(operationContext.EntityType.AsRuntimeTypeInfo());
            return entity;
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