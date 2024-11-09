// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PeersChangedMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Pipes.Endpoints
{
    using Kephas.Orchestration;

    /// <summary>
    /// Message for peers changed notification.
    /// </summary>
    public class PeersChangedMessage
    {
        /// <summary>
        /// Gets or sets the ID of the application instance sending this message.
        /// </summary>
        public string AppInstanceId { get; set; }

        /// <summary>
        /// Gets or sets the live apps.
        /// </summary>
        public IRuntimeAppInfo[]? Apps { get; set; }
    }
}