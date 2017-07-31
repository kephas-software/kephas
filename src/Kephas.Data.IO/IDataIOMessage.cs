// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataIOMessage.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for data I/O messages.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO
{
    using System;

    /// <summary>
    /// Contract for data exchange messages.
    /// </summary>
    public interface IDataIOMessage
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        string Message { get; }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        DateTimeOffset Timestamp { get; }
    }
}