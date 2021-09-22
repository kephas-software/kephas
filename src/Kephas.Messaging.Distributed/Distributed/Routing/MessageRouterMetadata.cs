// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageRouterMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message router metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed.Routing
{
    using System;
    using System.Collections.Generic;
    using Kephas.Collections;
    using Kephas.Services;

    /// <summary>
    /// A message router metadata.
    /// </summary>
    public class MessageRouterMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRouterMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public MessageRouterMetadata(IDictionary<string, object?>? metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.ReceiverMatch = (string)metadata.TryGetValue(nameof(this.ReceiverMatch));
            this.ReceiverMatchProviderType = (Type)metadata.TryGetValue(nameof(this.ReceiverMatchProviderType));
            this.IsFallback = (bool)metadata.TryGetValue(nameof(this.IsFallback), false);
            this.IsOptional = (bool)metadata.TryGetValue(nameof(this.IsOptional), false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRouterMetadata"/> class.
        /// </summary>
        /// <param name="receiverMatch">The receiver match expression.</param>
        /// <param name="isFallback">Optional. True if this router is fallback, false if not.</param>
        /// <param name="isOptional">Optional. True if the router is optional. Optional routers which cannot be initialized are simply ignored.</param>
        /// <param name="processingPriority">Optional. The processing priority.</param>
        /// <param name="overridePriority">Optional. The override priority.</param>
        /// <param name="serviceName">Optional. Name of the service.</param>
        public MessageRouterMetadata(string receiverMatch, bool isFallback = false, bool isOptional = false, Priority processingPriority = 0, Priority overridePriority = 0, string? serviceName = null)
            : base(processingPriority, overridePriority, serviceName)
        {
            this.ReceiverMatch = receiverMatch;
            this.IsFallback = isFallback;
            this.IsOptional = isOptional;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRouterMetadata"/> class.
        /// </summary>
        /// <param name="receiverMatchProviderType">The type of the receiver match provider.</param>
        /// <param name="isFallback">Optional. True if this router is fallback, false if not.</param>
        /// <param name="isOptional">Optional. True if the router is optional. Optional routers which
        ///                          cannot be initialized are simply ignored.</param>
        /// <param name="processingPriority">Optional. The processing priority.</param>
        /// <param name="overridePriority">Optional. The override priority.</param>
        /// <param name="serviceName">Optional. Name of the service.</param>
        public MessageRouterMetadata(Type receiverMatchProviderType, bool isFallback = false, bool isOptional = false, Priority processingPriority = 0, Priority overridePriority = 0, string? serviceName = null)
            : base(processingPriority, overridePriority, serviceName)
        {
            this.ReceiverMatchProviderType = receiverMatchProviderType;
            this.IsFallback = isFallback;
            this.IsOptional = isOptional;
        }

        /// <summary>
        /// Gets the receiver URL regular expression.
        /// </summary>
        /// <value>
        /// The receiver URL regular expression.
        /// </value>
        public string ReceiverMatch { get; }

        /// <summary>
        /// Gets the type of the receiver match provider.
        /// </summary>
        /// <value>
        /// The type of the receiver match provider.
        /// </value>
        public Type ReceiverMatchProviderType { get; }

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
