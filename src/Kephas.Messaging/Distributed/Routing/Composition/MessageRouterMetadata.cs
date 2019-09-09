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
            this.IsFallback = (bool)metadata.TryGetValue(nameof(this.IsFallback), false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRouterMetadata"/> class.
        /// </summary>
        /// <param name="receiverUrlRegex">The receiver URL regular expression.</param>
        /// <param name="isFallback">Optional. True if this router is fallback, false if not.</param>
        /// <param name="processingPriority">Optional. The processing priority.</param>
        /// <param name="overridePriority">Optional. The override priority.</param>
        /// <param name="serviceName">Optional. Name of the service.</param>
        public MessageRouterMetadata(string receiverUrlRegex, bool isFallback = false, int processingPriority = 0, int overridePriority = 0, string serviceName = null)
            : base(processingPriority, overridePriority, serviceName)
        {
            this.ReceiverUrlRegex = receiverUrlRegex;
            this.IsFallback = isFallback;
        }

        /// <summary>
        /// Gets the receiver URL regular expression.
        /// </summary>
        /// <value>
        /// The receiver URL regular expression.
        /// </value>
        public string ReceiverUrlRegex { get; }

        /// <summary>
        /// Gets a value indicating whether this router is the fallback router.
        /// </summary>
        /// <value>
        /// True if this router is fallback, false if not.
        /// </value>
        public bool IsFallback { get; }
    }
}
