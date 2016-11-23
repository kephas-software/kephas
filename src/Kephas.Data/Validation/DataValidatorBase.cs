// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataValidatorBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data validator base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Validation
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Resources;

    /// <summary>
    /// Base class for data validators.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    public abstract class DataValidatorBase<TEntity> : IDataValidator<TEntity>
    {
        /// <summary>
        /// Validates the provided instance asynchronously.
        /// </summary>
        /// <param name="obj">The object being validated.</param>
        /// <param name="operationContext">Context for the validation operation.</param>
        /// <param name="cancellationToken">(Optional) the cancellation token.</param>
        /// <returns>
        /// A promise of a <see cref="IDataValidationResult"/>.
        /// </returns>
        public abstract Task<IDataValidationResult> ValidateAsync(
            TEntity obj,
            IDataOperationContext operationContext,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Validates the provided instance asynchronously.
        /// </summary>
        /// <param name="obj">The object being validated.</param>
        /// <param name="operationContext">Context for the validation operation.</param>
        /// <param name="cancellationToken">(Optional) the cancellation token.</param>
        /// <returns>
        /// A promise of a <see cref="IDataValidationResult"/>.
        /// </returns>
        public Task<IDataValidationResult> ValidateAsync(
            object obj,
            IDataOperationContext operationContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!(obj is TEntity))
            {
                throw new DataValidationException(obj, new DataValidationResult(string.Format(Strings.DataValidator_MismatchedEntityType_Exception, typeof(TEntity), obj.GetType())));
            }

            return this.ValidateAsync((TEntity)obj, operationContext, cancellationToken);
        }
    }
}