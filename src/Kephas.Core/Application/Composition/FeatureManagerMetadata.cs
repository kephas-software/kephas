// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureManagerMetadata.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the feature manager metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Composition
{
    using System.Collections.Generic;

    using Kephas.Application.AttributedModel;
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
        /// Gets information describing the feature.
        /// </summary>
        /// <value>
        /// Information describing the feature.
        /// </value>
        public FeatureInfo FeatureInfo { get; internal set; }
    }
}