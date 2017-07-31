// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataIOMessage.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    }
}