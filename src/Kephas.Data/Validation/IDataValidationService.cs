// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataValidationService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataValidationService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Validation
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Service contract for data validation services.
    /// </summary>
    [SharedAppServiceContract]
    public interface IDataValidationService
    {
        /// <summary>
        /// Validates the provided instance asynchronously.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="operationContext">Context for the operation.</param>
        /// <param name="cancellationToken">(Optional) the cancellation token.</param>
        /// <returns>
        /// A promise of a <see cref="IDataValidationResult"/>.
        /// </returns>
        Task<IDataValidationResult> ValidateAsync(object obj, IDataOperationContext operationContext, CancellationToken cancellationToken = default(CancellationToken));
    }
}