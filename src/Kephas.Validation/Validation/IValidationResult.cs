// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValidationResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataValidationResult interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Validation
{
    using Kephas.ExceptionHandling;
    using Kephas.Operations;

    /// <summary>
    /// Interface for validation result.
    /// </summary>
    public interface IValidationResult : IOperationResult, IEnumerable<IValidationMessage>
    {
        /// <summary>
        /// Gets a value indicating whether the validation result has errors.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the validation result contains errors, <c>false</c> if not.
        /// </returns>
        new bool HasErrors() => this.Any(i => i.Severity == SeverityLevel.Error);

        /// <summary>
        /// Indicates whether the result has errors.
        /// </summary>
        /// <returns>
        /// A TResult.
        /// </returns>
        bool IOperationResult.HasErrors() => this.HasErrors();

        /// <summary>
        /// Gets only the errors items from this validation result.
        /// </summary>
        /// <returns>
        /// An enumeration of errors.
        /// </returns>
        IEnumerable<IValidationMessage> GetErrors() => this.Where(i => i.Severity == SeverityLevel.Error);
    }
}