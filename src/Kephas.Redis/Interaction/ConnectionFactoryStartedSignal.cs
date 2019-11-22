// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedisClientStartedSignal.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the redis client started signal class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Redis.Interaction
{
    using Kephas.ExceptionHandling;
    using Kephas.Interaction;

    /// <summary>
    /// The Redis connection factory started signal. Issued after the factory completed initialization.
    /// </summary>
    public class ConnectionFactoryStartedSignal : ISignal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionFactoryStartedSignal"/> class.
        /// </summary>
        /// <param name="message">Optional. The message.</param>
        /// <param name="severity">Optional. The severity.</param>
        public ConnectionFactoryStartedSignal(string message = null, SeverityLevel severity = SeverityLevel.Info)
        {
            this.Message = message ?? "Redis client initialized.";
            this.Severity = severity;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; }

        /// <summary>
        /// Gets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        public SeverityLevel Severity { get; }
    }
}
