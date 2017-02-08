// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataBehaviorBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   An entity behavior base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Behaviors
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Capabilities;
    using Kephas.Data.Validation;

    /// <summary>
    /// An entity behavior base.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    public abstract class DataBehaviorBase<TEntity> : IDataBehavior<TEntity>, IOnPersistBehavior, IOnInitializeBehavior, IOnValidateBehavior
    {
        /// <summary>
        /// Callback invoked upon entity initialization.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="operationContext">The operation context.</param>
        public virtual void Initialize(TEntity entity, IDataOperationContext operationContext)
        {
        }

        /// <summary>
        /// Callback invoked before an entity is being persisted.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="operationContext">The operation context.</param>
        public virtual void BeforePersist(TEntity entity, IDataOperationContext operationContext)
        {
        }

        /// <summary>
        /// Callback invoked after an entity has been persisted.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="operationContext">The operation context.</param>
        public virtual void AfterPersist(TEntity entity, IDataOperationContext operationContext)
        {
        }

        /// <summary>
        /// Callback invoked after upon entity validation.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <returns>
        /// An <see cref="IDataValidationResult"/>.
        /// </returns>
        public virtual IDataValidationResult Validate(TEntity entity, IDataOperationContext operationContext)
        {
            return DataValidationResult.Success;
        }

        /// <summary>
        /// Async callback invoked before an entity is being persisted.
        /// </summary>
        /// <param name="obj">The object to be persisted.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public Task BeforePersistAsync(
            object obj,
            IDataOperationContext operationContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            this.BeforePersist((TEntity)obj, operationContext);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Async callback invoked after an entity was persisted.
        /// </summary>
        /// <param name="obj">The persisted object.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public Task AfterPersistAsync(
            object obj,
            IDataOperationContext operationContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            this.AfterPersist((TEntity)obj, operationContext);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Initializes the entity asynchronously.
        /// </summary>
        /// <param name="obj">The object to be initialized.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public Task InitializeAsync(
            object obj,
            IDataOperationContext operationContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            this.Initialize((TEntity)obj, operationContext);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Validates the provided instance asynchronously.
        /// </summary>
        /// <param name="obj">The object being validated.</param>
        /// <param name="operationContext">Context for the validation operation.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of a <see cref="IDataValidationResult" />
        /// .
        /// </returns>
        public Task<IDataValidationResult> ValidateAsync(
            object obj,
            IDataOperationContext operationContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(this.Validate((TEntity)obj, operationContext));
        }

        /// <summary>
        /// Gets the entity change state.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <returns>
        /// The entity change state.
        /// </returns>
        protected ChangeState GetChangeState(TEntity entity, IDataOperationContext operationContext)
        {
            return operationContext.DataContext.TryGetCapability<IChangeStateTrackable>(entity, operationContext)?.ChangeState ?? ChangeState.NotChanged;
        }
    }
}
