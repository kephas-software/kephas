// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataValidator.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataValidator interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Validation
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Application service contract for data validators.
    /// </summary>
    public interface IDataValidator
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
        Task<IDataValidationResult> ValidateAsync(object obj, IDataOperationContext operationContext, CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// Generic application service contract for data validators.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity being validated.</typeparam>
    [AppServiceContract(ContractType = typeof(IDataValidator))]
    public interface IDataValidator<in TEntity> : IDataValidator
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
        Task<IDataValidationResult> ValidateAsync(TEntity obj, IDataOperationContext operationContext, CancellationToken cancellationToken = default(CancellationToken));
    }
}