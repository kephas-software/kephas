// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataValidationResultItem.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataValidationResultItem interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Validation
{
    using Kephas.Operations;

    /// <summary>
    /// Interface for validation result item.
    /// </summary>
    public interface IDataValidationResultItem : IOperationMessage
    {
        /// <summary>
        /// Gets the validation result severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        DataValidationSeverity Severity { get; }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <value>
        /// The name of the member.
        /// </value>
        string? MemberName { get; }
    }
}