// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionCreateBehaviorMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Connectivity.Behaviors
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Services;

    /// <summary>
    /// The <see cref="IConnectionCreateBehavior"/> service metadata.
    /// </summary>
    public class ConnectionCreateBehaviorMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionCreateBehaviorMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public ConnectionCreateBehaviorMetadata(IDictionary<string, object?>? metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.ConnectionKind = (string[]?)metadata.TryGetValue(nameof(this.ConnectionKind)) ?? Array.Empty<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionCreateBehaviorMetadata"/> class.
        /// </summary>
        /// <param name="connectionKind">The supported connection kind.</param>
        /// <param name="processingPriority">Optional. The processing priority.</param>
        /// <param name="overridePriority">Optional. The override priority.</param>
        /// <param name="serviceName">Optional. Name of the service.</param>
        public ConnectionCreateBehaviorMetadata(string[] connectionKind, Priority processingPriority = 0, Priority overridePriority = 0, string? serviceName = null)
            : base(processingPriority, overridePriority, serviceName)
        {
            this.ConnectionKind = connectionKind ?? throw new ArgumentNullException(nameof(connectionKind));
        }

        /// <summary>
        /// Gets the templateKind array.
        /// </summary>
        /// <value>
        /// The templateKind array.
        /// </value>
        public string[] ConnectionKind { get; } = Array.Empty<string>();
    }
}
