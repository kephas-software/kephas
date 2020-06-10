// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisterPeerMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Pipes.Endpoints
{
    /// <summary>
    /// Message for registering a peer.
    /// </summary>
    public class RegisterPeerMessage // : IMessage // do not register as message, as this is something internal for the pipes infrastructure
    {
        /// <summary>
        /// Gets or sets the caller application instance ID.
        /// </summary>
        public string? AppInstanceId { get; set; }

        /// <summary>
        /// Gets or sets the name of the channel.
        /// </summary>
        public string? ChannelName { get; set; }

        /// <summary>
        /// Gets or sets the name of the server hosting the pipe.
        /// </summary>
        public string? ServerName { get; set; }
    }

    /// <summary>
    /// Response message for peer registration.
    /// </summary>
    public class RegisterPeerResponseMessage // : IMessage // do not register as message, as this is something internal for the pipes infrastructure
    {
        /// <summary>
        /// Gets or sets the peers.
        /// </summary>
        public RegisterPeerMessage[] Peers { get; set; }
    }
}