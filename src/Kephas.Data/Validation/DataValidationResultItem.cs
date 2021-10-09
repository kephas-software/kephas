// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataValidationResultItem.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data validation result item class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Validation
{
    using System;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Operations;

    /// <summary>
    /// A data validation result item.
    /// </summary>
    public class DataValidationResultItem : OperationMessage, IDataValidationResultItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataValidationResultItem"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="memberName">Optional. the member name.</param>
        /// <param name="severity">Optional. the severity.</param>
        public DataValidationResultItem(string message, string? memberName = null, DataValidationSeverity severity = DataValidationSeverity.Error)
            : base(message)
        {
            message = message ?? throw new ArgumentNullException(nameof(message));

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
        /// Gets the name of the member.
        /// </summary>
        /// <value>
        /// The name of the member.
        /// </value>
        public string? MemberName { get; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var member = this.MemberName == null ? null : $": {this.MemberName}";
            return $"{base.ToString()} ({this.Severity}{member})";
        }
    }
}