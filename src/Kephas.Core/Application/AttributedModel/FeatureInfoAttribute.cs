// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureInfoAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the feature information attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.AttributedModel
{
    using System;

    using Kephas.Composition.Metadata;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Attribute providing feature information.
    /// </summary>
    public class FeatureInfoAttribute : Attribute, IMetadataValue<FeatureInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureInfoAttribute"/> class.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="dependencies">The feature dependencies (optional).</param>
        public FeatureInfoAttribute(string feature, string[] dependencies = null)
        {
            Requires.NotNullOrEmpty(feature, nameof(feature));

            this.Value = new FeatureInfo(feature, dependencies);
        }

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        object IMetadataValue.Value => this.Value;

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        public FeatureInfo Value { get; }
    }

}