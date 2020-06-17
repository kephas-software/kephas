// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HandshakeMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Pipes.Endpoints
{
    /// <summary>
    /// Message for peer hand shaking.
    /// </summary>
    public class HandshakeMessage
    {
        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        public string? Target { get; set; }
    }
}