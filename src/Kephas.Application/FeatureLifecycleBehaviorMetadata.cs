// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureLifecycleBehaviorMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the feature lifecycle behavior metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Services;

    /// <summary>
    /// A feature lifecycle behavior metadata.
    /// </summary>
    public class FeatureLifecycleBehaviorMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureLifecycleBehaviorMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public FeatureLifecycleBehaviorMetadata(IDictionary<string, object?>? metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.Target = (FeatureRef?)metadata.TryGetValue(nameof(this.Target));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureLifecycleBehaviorMetadata"/> class.
        /// </summary>
        /// <param name="feature">Optional. The feature for which the behavior applies.</param>
        /// <param name="version">Optional. The version.</param>
        /// <param name="processingPriority">Optional. The processing priority.</param>
        /// <param name="overridePriority">Optional. The override priority.</param>
        public FeatureLifecycleBehaviorMetadata(string? feature = null, string? version = null, Priority processingPriority = 0, Priority overridePriority = 0)
            : base(processingPriority, overridePriority)
        {
            if (!string.IsNullOrEmpty(feature))
            {
                this.Target = new FeatureRef(feature, version);
            }
        }

        /// <summary>
        /// Gets or sets the feature reference for which the behavior applies.
        /// </summary>
        /// <value>
        /// The feature reference for which the behavior applies.
        /// </value>
        public FeatureRef? Target { get; set; }
    }
}