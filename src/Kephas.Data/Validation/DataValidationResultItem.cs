// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataValidationResultItem.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data validation result item class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Validation
{
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A data validation result item.
    /// </summary>
    public class DataValidationResultItem : IDataValidationResultItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataValidationResultItem"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="memberName">Optional. the member name.</param>
        /// <param name="severity">Optional. the severity.</param>
        public DataValidationResultItem(string message, string memberName = null, DataValidationSeverity severity = DataValidationSeverity.Error)
        {
            Requires.NotNull(message, nameof(message));

            this.Message = message;
            this.MemberName = memberName;
            this.Severity = severity;
        }

        /// <summary>
        /// Gets the validation result severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        public DataValidationSeverity Severity { get; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <value>
        /// The name of the member.
        /// </value>
        public string MemberName { get; }
    }
}