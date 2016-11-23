// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOnValidateBehavior.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IOnValidateBehavior interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Validation
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Contract for the behavior invoked upon entity validation.
    /// </summary>
    public interface IOnValidateBehavior
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
}