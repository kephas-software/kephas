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

    /// <summary>
    /// An operation message.
    /// </summary>
    public class OperationMessage : IOperationMessage, IDataFormattable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public OperationMessage(string message)
        {
            this.Message = message;
            this.Timestamp = DateTimeOffset.Now;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; }

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
            return $"[{this.Timestamp:s}] {this.Message}";
        }

        /// <summary>
        /// Converts this object to a serialization friendly representation.
        /// </summary>
        /// <param name="context">Optional. The formatting context.</param>
        /// <returns>A serialization friendly object representing this object.</returns>
        public virtual object ToData(IDataFormattingContext? context = null)
        {
            return this.ToString();
        }
    }
}