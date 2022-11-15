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
    using Kephas.Services;

    /// <summary>
    /// Attribute providing feature information.
    /// </summary>
    public class FeatureInfoAttribute : Attribute, IMetadataValue<FeatureInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureInfoAttribute"/> class.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="version">Optional. The feature version.</param>
        /// <param name="isRequired">Optional. True if this feature is required, false (default) if not.</param>
        /// <param name="dependencies">Optional. The feature dependencies.</param>
        /// <param name="targetApps">Optional. The target applications where the feature will be loaded.</param>
        public FeatureInfoAttribute(string feature, string? version = null, bool isRequired = false, string[]? dependencies = null, string[]? targetApps = null)
        {
            feature = feature ?? throw new ArgumentNullException(nameof(feature));

            this.Value = new FeatureInfo(feature, version, isRequired, dependencies, targetApps);
        }

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        public FeatureInfo Value { get; }
    }
}