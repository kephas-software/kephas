// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataValidationResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data validation result class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Validation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Operations;

    /// <summary>
    /// Encapsulates the result of a data validation.
    /// </summary>
    public class DataValidationResult : OperationResult, IDataValidationResult
    {
        /// <summary>
        /// The validation result indicating that the validation succeeded without any issues.
        /// </summary>
        public static readonly DataValidationResult Success = new DataValidationResult();

        /// <summary>
        /// Initializes a new instance of the <see cref="DataValidationResult"/> class.
        /// </summary>
        public DataValidationResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataValidationResult"/>
        /// class by adding the items to the result.
        /// </summary>
        /// <param name="items">The items to add.</param>
        public DataValidationResult(params IDataValidationResultItem[] items)
        {
            items = items ?? throw new ArgumentNullException(nameof(items));

            this.Add(items);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataValidationResult"/> class
        /// by adding a new item with the provided parameters.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="memberName">The name of the member.</param>
        /// <param name="severity">The severity.</param>
        public DataValidationResult(string message, string? memberName = null, DataValidationSeverity severity = DataValidationSeverity.Error)
        {
            message = message ?? throw new ArgumentNullException(nameof(message));

            this.Add(message, memberName, severity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataValidationResult"/> class
        /// by adding a new item with the provided parameters.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="memberName">The name of the member.</param>
        /// <param name="severity">The severity.</param>
        public DataValidationResult(Exception exception, string? memberName = null, DataValidationSeverity severity = DataValidationSeverity.Error)
        {
            exception = exception ?? throw new ArgumentNullException(nameof(exception));

            this.Add(exception.Message, memberName, severity);
            this.MergeException(exception);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        public IEnumerator<IDataValidationResultItem> GetEnumerator()
        {
            return this.Messages.OfType<IDataValidationResultItem>().GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Messages.OfType<IDataValidationResultItem>().GetEnumerator();
        }

        /// <summary>
        /// Adds the items to the validation result.
        /// </summary>
        /// <param name="items">The items to add.</param>
        /// <returns>
        /// This <see cref="DataValidationResult"/>.
        /// </returns>
        public DataValidationResult Add(params IDataValidationResultItem[] items)
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
        /// <param name="severity">The severity (optional). Default value is <see cref="DataValidationSeverity.Error"/></param>
        /// <returns>
        /// This <see cref="DataValidationResult"/>.
        /// </returns>
        public DataValidationResult Add(string message, string? memberName = null, DataValidationSeverity severity = DataValidationSeverity.Error)
        {
            message = message ?? throw new ArgumentNullException(nameof(message));

            this.MergeMessage(new DataValidationResultItem(message, memberName, severity));
            return this;
        }
    }
}