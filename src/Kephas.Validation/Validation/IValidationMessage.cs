// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValidationMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataValidationResultItem interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Validation
{
    using Kephas.ExceptionHandling;
    using Kephas.Operations;

    /// <summary>
    /// Interface for validation result item.
    /// </summary>
    public interface IValidationMessage : IOperationMessage, ISeverityQualifiedNotification
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        new string Message { get; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        string IOperationMessage.Message => this.Message;

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        string ISeverityQualifiedNotification.Message => this.Message;

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <value>
        /// The name of the member.
        /// </value>
        string? MemberName { get; }
    }
}