// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullDataValidationService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the null validation service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Validation
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// A null validation service.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullDataValidationService : IDataValidationService
    {
        /// <summary>
        /// Validates the provided instance asynchronously.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">(Optional) the cancellation token.</param>
        /// <returns>
        /// A promise of a <see cref="IDataValidationResult"/>.
        /// </returns>
        public Task<IDataValidationResult> ValidateAsync(object obj, IContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult((IDataValidationResult)DataValidationResult.Success);
        }
    }
}