// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JoinPeerMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Pipes.Endpoints
{
    /// <summary>
    /// Message for joining a peer.
    /// </summary>
    public class JoinPeerMessage
    {
        /// <summary>
        /// Gets or sets the ID of the application instance joining the system.
        /// </summary>
        public string AppInstanceId { get; set; }
    }
}