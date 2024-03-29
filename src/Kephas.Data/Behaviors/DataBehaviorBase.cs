﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataBehaviorBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    using Kephas.Logging;
    using Kephas.Threading.Tasks;
    using Kephas.Validation;

    /// <summary>
    /// An entity behavior base.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    public abstract class DataBehaviorBase<TEntity> : Loggable, IDataBehavior<TEntity>, IOnPersistBehavior, IOnInitializeBehavior, IOnValidateBehavior
    {
        /// <summary>
        /// Callback invoked upon entity initialization.
        /// </summary>
        /// <param name="entity">The entity to be initialized.</param>
        /// <param name="entityEntry">The entity entry.</param>
        /// <param name="operationContext">The operation context.</param>
        public virtual void Initialize(TEntity entity, IEntityEntry entityEntry, IDataOperationContext operationContext)
        {
        }

        /// <summary>
        /// Callback invoked before an entity is being persisted.
        /// </summary>
        /// <param name="entity">The entity to be persisted.</param>
        /// <param name="entityEntry">The entity entry.</param>
        /// <param name="operationContext">The operation context.</param>
        public virtual void BeforePersist(TEntity entity, IEntityEntry entityEntry, IDataOperationContext operationContext)
        {
        }

        /// <summary>
        /// Callback invoked after an entity has been persisted.
        /// </summary>
        /// <param name="entity">The entity to be persisted.</param>
        /// <param name="entityEntry">The entity entry.</param>
        /// <param name="operationContext">The operation context.</param>
        public virtual void AfterPersist(TEntity entity, IEntityEntry entityEntry, IDataOperationContext operationContext)
        {
        }

        /// <summary>
        /// Callback invoked after upon entity validation.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="entityEntry">The entity entry.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <returns>
        /// An <see cref="IValidationResult"/>.
        /// </returns>
        public virtual IValidationResult Validate(TEntity entity, IEntityEntry entityEntry, IDataOperationContext operationContext)
        {
            return ValidationResult.Success;
        }

        /// <summary>
        /// Callback invoked before an entity is being persisted.
        /// </summary>
        /// <param name="entity">The entity to be persisted.</param>
        /// <param name="entityEntry">The entity entry.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task IOnPersistBehavior.BeforePersistAsync(object entity, IEntityEntry entityEntry, IDataOperationContext operationContext, CancellationToken cancellationToken)
        {
            return this.BeforePersistAsync((TEntity)entity, entityEntry, operationContext, cancellationToken);
        }

        /// <summary>
        /// Callback invoked before an entity is being persisted.
        /// </summary>
        /// <param name="entity">The entity to be persisted.</param>
        /// <param name="entityEntry">The entity entry.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public virtual Task BeforePersistAsync(TEntity entity, IEntityEntry entityEntry, IDataOperationContext operationContext, CancellationToken cancellationToken)
        {
            this.BeforePersist(entity, entityEntry, operationContext);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Callback invoked after an entity was persisted.
        /// </summary>
        /// <param name="entity">The entity to be persisted.</param>
        /// <param name="entityEntry">The entity entry.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task IOnPersistBehavior.AfterPersistAsync(object entity, IEntityEntry entityEntry, IDataOperationContext operationContext, CancellationToken cancellationToken)
        {
            return this.AfterPersistAsync((TEntity)entity, entityEntry, operationContext, cancellationToken);
        }

        /// <summary>
        /// Callback invoked after an entity was persisted.
        /// </summary>
        /// <param name="entity">The entity to be persisted.</param>
        /// <param name="entityEntry">The entity entry.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public virtual Task AfterPersistAsync(TEntity entity, IEntityEntry entityEntry, IDataOperationContext operationContext, CancellationToken cancellationToken)
        {
            this.AfterPersist(entity, entityEntry, operationContext);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Initializes the entity asynchronously.
        /// </summary>
        /// <param name="entity">The entity to be initialized.</param>
        /// <param name="entityEntry">The entity entry.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task IOnInitializeBehavior.InitializeAsync(object entity, IEntityEntry entityEntry, IDataOperationContext operationContext, CancellationToken cancellationToken)
        {
            return this.InitializeAsync((TEntity)entity, entityEntry, operationContext, cancellationToken);
        }

        /// <summary>
        /// Initializes the entity asynchronously.
        /// </summary>
        /// <param name="entity">The entity to be initialized.</param>
        /// <param name="entityEntry">The entity entry.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public virtual Task InitializeAsync(TEntity entity, IEntityEntry entityEntry, IDataOperationContext operationContext, CancellationToken cancellationToken)
        {
            this.Initialize(entity, entityEntry, operationContext);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Validates the provided instance asynchronously.
        /// </summary>
        /// <param name="entity">The entity to be validated.</param>
        /// <param name="entityEntry">The entity entry.</param>
        /// <param name="operationContext">Context for the validation operation.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of a <see cref="IValidationResult"/>.
        /// </returns>
        Task<IValidationResult> IOnValidateBehavior.ValidateAsync(object entity, IEntityEntry entityEntry, IDataOperationContext operationContext, CancellationToken cancellationToken)
        {
            return this.ValidateAsync((TEntity)entity, entityEntry, operationContext, cancellationToken);
        }

        /// <summary>
        /// Validates the provided instance asynchronously.
        /// </summary>
        /// <param name="entity">The entity to be validated.</param>
        /// <param name="entityEntry">The entity entry.</param>
        /// <param name="operationContext">Context for the validation operation.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of a <see cref="IValidationResult"/>.
        /// </returns>
        public virtual Task<IValidationResult> ValidateAsync(TEntity entity, IEntityEntry entityEntry, IDataOperationContext operationContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(this.Validate(entity, entityEntry, operationContext));
        }
    }
}
