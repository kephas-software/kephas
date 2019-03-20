// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOnValidateBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IOnValidateBehavior interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Validation
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Capabilities;

    /// <summary>
    /// Contract for the behavior invoked upon entity validation.
    /// </summary>
    public interface IOnValidateBehavior
    {
        /// <summary>
        /// Validates the provided instance asynchronously.
        /// </summary>
        /// <param name="entity">The entity to be validated.</param>
        /// <param name="entityEntry">The entity entry.</param>
        /// <param name="operationContext">Context for the validation operation.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of a <see cref="IDataValidationResult"/>.
        /// </returns>
        Task<IDataValidationResult> ValidateAsync(object entity, IEntityEntry entityEntry, IDataOperationContext operationContext, CancellationToken cancellationToken = default);
    }
}