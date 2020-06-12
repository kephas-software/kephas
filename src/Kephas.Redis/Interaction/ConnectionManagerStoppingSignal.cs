// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionManagerStoppingSignal.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the redis client stopping signal class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Redis.Interaction
{
    using Kephas.ExceptionHandling;
    using Kephas.Interaction;

    /// <summary>
    /// The Redis connection manager stopping signal. Issued before the manager starts finalization/disposal.
    /// </summary>
    public class ConnectionManagerStoppingSignal : SignalBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionManagerStoppingSignal"/> class.
        /// </summary>
        /// <param name="message">Optional. The message.</param>
        /// <param name="severity">Optional. The severity.</param>
        public ConnectionManagerStoppingSignal(string? message = null, SeverityLevel severity = SeverityLevel.Info)
            : base(message ?? "The Redis connection manager is stopping.", severity)
        {
        }
    }
}
