// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data validation result item class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Validation
{
    using Kephas.ExceptionHandling;
    using Kephas.Operations;

    /// <summary>
    /// A data validation result item.
    /// </summary>
    public class ValidationMessage : OperationMessage, IValidationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationMessage"/> class.
        /// </summary>
        /// <param name="message">The exception.</param>
        /// <param name="memberName">Optional. the member name.</param>
        /// <param name="severity">Optional. the severity.</param>
        public ValidationMessage(string message, string? memberName = null, SeverityLevel severity = SeverityLevel.Error)
            : base(message ?? throw new ArgumentNullException(nameof(message)))
        {
            this.MemberName = memberName;
            this.Severity = severity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationMessage"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="memberName">Optional. the member name.</param>
        /// <param name="severity">Optional. the severity.</param>
        public ValidationMessage(Exception exception, string? memberName = null, SeverityLevel severity = SeverityLevel.Error)
            : base(exception ?? throw new ArgumentNullException(nameof(exception)))
        {
            this.MemberName = memberName;
            this.Severity = severity;
        }

        /// <summary>
        /// Gets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        public SeverityLevel Severity { get; }

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