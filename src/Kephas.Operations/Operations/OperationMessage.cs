// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the operation message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Operations
{
    using System;

    using Kephas.Data.Formatting;
    using Kephas.Dynamic;
    using Kephas.ExceptionHandling;

    /// <summary>
    /// An operation message.
    /// </summary>
    public class OperationMessage : IOperationMessage, IDataFormattable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">Optional. The exception.</param>
        public OperationMessage(string message, Exception? exception = null)
        {
            this.Message = message;
            this.Exception = exception;
            this.Timestamp = DateTimeOffset.Now;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationMessage"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public OperationMessage(Exception exception)
            : this(exception?.Message ?? string.Empty, exception ?? throw new ArgumentNullException(nameof(exception)))
        {
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; }

        /// <summary>
        /// Gets the exception, if any.
        /// </summary>
        public Exception? Exception { get; }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public DateTimeOffset Timestamp { get; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            var exceptionMessage = this.Exception is null ? null : $" ({this.Exception.GetType()})";
            return $"[{this.Timestamp:s}] {this.Message}{exceptionMessage}";
        }

        /// <summary>
        /// Converts this object to a serialization friendly representation.
        /// </summary>
        /// <param name="context">Optional. The formatting context.</param>
        /// <returns>A serialization friendly object representing this object.</returns>
        object? IDataFormattable.ToData(object? context)
        {
            return this.Exception is null
                ? this.ToString()
                : new Expando
                {
                    [nameof(this.Timestamp)] = this.Timestamp,
                    [nameof(this.Message)] = this.Message,
                    [nameof(this.Exception)] = new ExceptionData(this.Exception),
                };
        }
    }
}