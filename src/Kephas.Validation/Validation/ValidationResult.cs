// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data validation result class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Validation
{
    using System.Collections;

    using Kephas.Collections;
    using Kephas.ExceptionHandling;
    using Kephas.Operations;

    /// <summary>
    /// Encapsulates the result of a data validation.
    /// </summary>
    public record ValidationResult : OperationResult, IValidationResult
    {
        /// <summary>
        /// The validation result indicating that the validation succeeded without any issues.
        /// </summary>
        public static readonly ValidationResult Success = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        public ValidationResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/>
        /// class by adding the items to the result.
        /// </summary>
        /// <param name="items">The items to add.</param>
        public ValidationResult(params IValidationMessage[] items)
        {
            items = items ?? throw new ArgumentNullException(nameof(items));

            this.Add(items);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class
        /// by adding a new item with the provided parameters.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="memberName">The name of the member.</param>
        /// <param name="severity">The severity.</param>
        public ValidationResult(string message, string? memberName = null, SeverityLevel severity = SeverityLevel.Error)
        {
            message = message ?? throw new ArgumentNullException(nameof(message));

            this.Add(message, memberName, severity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class
        /// by adding a new item with the provided parameters.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="memberName">The name of the member.</param>
        /// <param name="severity">The severity.</param>
        public ValidationResult(Exception exception, string? memberName = null, SeverityLevel severity = SeverityLevel.Error)
        {
            exception = exception ?? throw new ArgumentNullException(nameof(exception));

            this.Add(exception, memberName, severity);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        public IEnumerator<IValidationMessage> GetEnumerator()
        {
            return this.Messages.OfType<IValidationMessage>().GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Messages.OfType<IValidationMessage>().GetEnumerator();
        }

        /// <summary>
        /// Adds the items to the validation result.
        /// </summary>
        /// <param name="items">The items to add.</param>
        /// <returns>
        /// This <see cref="ValidationResult"/>.
        /// </returns>
        public ValidationResult Add(params IValidationMessage[] items)
        {
            items = items ?? throw new ArgumentNullException(nameof(items));

            items.ForEach(m => this.MergeMessage(m));
            return this;
        }

        /// <summary>
        /// Adds a result item to the validation result.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="memberName">Name of the member (optional).</param>
        /// <param name="severity">The severity (optional). Default value is <see cref="SeverityLevel.Error"/>.</param>
        /// <returns>
        /// This <see cref="ValidationResult"/>.
        /// </returns>
        public ValidationResult Add(string message, string? memberName = null, SeverityLevel severity = SeverityLevel.Error)
        {
            message = message ?? throw new ArgumentNullException(nameof(message));

            this.MergeMessage(new ValidationMessage(message, memberName, severity));
            return this;
        }

        /// <summary>
        /// Adds a result item to the validation result.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="memberName">Name of the member (optional).</param>
        /// <param name="severity">The severity (optional). Default value is <see cref="SeverityLevel.Error"/>.</param>
        /// <returns>
        /// This <see cref="ValidationResult"/>.
        /// </returns>
        public ValidationResult Add(Exception exception, string? memberName = null, SeverityLevel severity = SeverityLevel.Error)
        {
            exception = exception ?? throw new ArgumentNullException(nameof(exception));

            this.MergeMessage(new ValidationMessage(exception, memberName, severity));
            return this;
        }
    }
}