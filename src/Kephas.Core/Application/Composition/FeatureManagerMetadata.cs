// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureManagerMetadata.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the feature manager metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Composition
{
    using System.Collections.Generic;

    using Kephas.Services.Composition;

    /// <summary>
    /// A feature manager metadata.
    /// </summary>
    public class FeatureManagerMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureManagerMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public FeatureManagerMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.FeatureInfo = this.GetMetadataValue<FeatureInfoAttribute, FeatureInfo>(metadata);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureManagerMetadata" /> class.
        /// </summary>
        /// <param name="featureInfo">Information describing the feature (optional).</param>
        /// <param name="processingPriority">The processing priority (optional).</param>
        /// <param name="overridePriority">The override priority (optional).</param>
        /// <param name="optionalService"><c>true</c> if the service is optional, <c>false</c> if not (optional).</param>
        public FeatureManagerMetadata(FeatureInfo featureInfo = null, int processingPriority = 0, int overridePriority = 0, bool optionalService = false)
            : base(processingPriority, overridePriority, optionalService)
        {
            this.FeatureInfo = featureInfo;
        }

        /// <summary>
        /// Gets information describing the feature.
        /// </summary>
        /// <value>
        /// Information describing the feature.
        /// </value>
        public FeatureInfo FeatureInfo { get; internal set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var deps = string.Join(",", this.FeatureInfo?.Dependencies ?? new string[0]);
            var feature = $"{this.FeatureInfo?.Name}({deps})";
            return $"{base.ToString()}, {feature}";
        }
    }
}