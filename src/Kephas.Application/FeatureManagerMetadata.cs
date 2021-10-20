// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureManagerMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the feature manager metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;

    using Kephas.Application.Reflection;
    using Kephas.Collections;
    using Kephas.Services;

    /// <summary>
    /// A feature manager metadata.
    /// </summary>
    public class FeatureManagerMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureManagerMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public FeatureManagerMetadata(IDictionary<string, object?>? metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.FeatureInfo = (FeatureInfo?)metadata.TryGetValue(nameof(this.FeatureInfo)) ?? FeatureInfo.FromMetadata(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureManagerMetadata" /> class.
        /// </summary>
        /// <param name="featureInfo">Optional. Information describing the feature.</param>
        /// <param name="processingPriority">Optional. The processing priority.</param>
        /// <param name="overridePriority">Optional. The override priority.</param>
        public FeatureManagerMetadata(FeatureInfo? featureInfo = null, Priority processingPriority = 0, Priority overridePriority = 0)
            : base(processingPriority, overridePriority)
        {
            this.FeatureInfo = featureInfo ?? FeatureInfo.FromMetadata(this);
        }

        /// <summary>
        /// Gets information describing the feature.
        /// </summary>
        /// <value>
        /// Information describing the feature.
        /// </value>
        public FeatureInfo FeatureInfo { get; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var deps = string.Join(",", this.FeatureInfo?.Dependencies ?? Array.Empty<string>());
            var feature = $"{this.FeatureInfo?.Name}({deps})";
            return $"{base.ToString()}, {feature}";
        }
    }
}