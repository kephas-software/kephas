// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataValidationResultItem.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataValidationResultItem interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Validation
{
    /// <summary>
    /// Interface for validation result item.
    /// </summary>
    public interface IDataValidationResultItem
    {
        /// <summary>
        /// Gets the validation result severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        DataValidationSeverity Severity { get; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        string Message { get; }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <value>
        /// The name of the member.
        /// </value>
        string MemberName { get; }
    }
}