// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageRouterMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message router metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed.Routing.Composition
{
    using System.Collections.Generic;
    using Kephas.Collections;
    using Kephas.Services.Composition;

    /// <summary>
    /// A message router metadata.
    /// </summary>
    public class MessageRouterMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRouterMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public MessageRouterMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.ReceiverUrlRegex = (string)metadata.TryGetValue(nameof(this.ReceiverUrlRegex));
            this.Channel = (string)metadata.TryGetValue(nameof(this.Channel));
            this.IsFallback = (bool)metadata.TryGetValue(nameof(this.IsFallback), false);
            this.IsOptional = (bool)metadata.TryGetValue(nameof(this.IsOptional), false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRouterMetadata"/> class.
        /// </summary>
        /// <param name="receiverUrlRegex">The receiver URL regular expression.</param>
        /// <param name="channel">Optional. The channel.</param>
        /// <param name="isFallback">Optional. True if this router is fallback, false if not.</param>
        /// <param name="isOptional">Optional. True if the router is optional. Optional routers which cannot be initialized are simply ignored.</param>
        /// <param name="processingPriority">Optional. The processing priority.</param>
        /// <param name="overridePriority">Optional. The override priority.</param>
        /// <param name="serviceName">Optional. Name of the service.</param>
        public MessageRouterMetadata(string receiverUrlRegex, string channel = null, bool isFallback = false, bool isOptional = false, int processingPriority = 0, int overridePriority = 0, string serviceName = null)
            : base(processingPriority, overridePriority, serviceName)
        {
            this.ReceiverUrlRegex = receiverUrlRegex;
            this.Channel = channel;
            this.IsFallback = isFallback;
            this.IsOptional = isOptional;
        }

        /// <summary>
        /// Gets the receiver URL regular expression.
        /// </summary>
        /// <value>
        /// The receiver URL regular expression.
        /// </value>
        public string ReceiverUrlRegex { get; }

        /// <summary>
        /// Gets or sets the channel.
        /// </summary>
        /// <value>
        /// The channel.
        /// </value>
        public string Channel { get; set; }

        /// <summary>
        /// Gets a value indicating whether this router is the fallback router.
        /// </summary>
        /// <value>
        /// True if this router is fallback, false if not.
        /// </value>
        public bool IsFallback { get; }

        /// <summary>
        /// Gets a value indicating whether the router is optional.
        /// Optional routers which cannot be initialized are simply ignored.
        /// </summary>
        /// <value>
        /// True if the router is optional, false otherwise.
        /// </value>
        public bool IsOptional { get; }
    }
}
