// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataIOMessage.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data exchange message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO
{
    using System;

    /// <summary>
    /// The data I/O message.
    /// </summary>
    public class DataIOMessage : IDataIOMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataIOMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public DataIOMessage(string message)
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
            return $"{this.Timestamp:s} {this.Message}";
        }
    }
}