// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the messaging settings class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging
{
    using Kephas.Messaging.Distributed;

    /// <summary>
    /// Messaging settings.
    /// </summary>
    public class MessagingSettings
    {
        /// <summary>
        /// Gets or sets the distributed messaging settings.
        /// </summary>
        /// <value>
        /// The distributed messaging settings.
        /// </value>
        public DistributedMessagingSettings Distributed { get; set; } = new DistributedMessagingSettings();
    }
}
