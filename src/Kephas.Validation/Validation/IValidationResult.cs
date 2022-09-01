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
        /// Gets only the errors items from this validation result.
        /// </summary>
        /// <returns>
        /// An enumeration of errors.
        /// </returns>
        IEnumerable<IValidationMessage> GetErrors() => this.Where(i => i.Severity == SeverityLevel.Error);
    }
}