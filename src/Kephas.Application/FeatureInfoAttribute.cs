// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureInfoAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the feature information attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using Kephas.Application.Reflection;
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
        /// <param name="version">The feature version (optional).</param>
        /// <param name="isRequired">True if this feature is required, false if not (optional).</param>
        /// <param name="dependencies">The feature dependencies (optional).</param>
        public FeatureInfoAttribute(string feature, string version = null, bool isRequired = false, string[] dependencies = null)
        {
            Requires.NotNullOrEmpty(feature, nameof(feature));

            this.Value = new FeatureInfo(feature, version, isRequired, dependencies);
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