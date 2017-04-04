// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the feature information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Provides information about an application feature.
    /// </summary>
    public class FeatureInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureInfo"/> class.
        /// </summary>
        /// <param name="featureName">The feature name.</param>
        /// <param name="dependencies">The feature dependencies (optional).</param>
        public FeatureInfo(string featureName, string[] dependencies = null)
        {
            Requires.NotNullOrEmpty(featureName, nameof(featureName));

            this.FeatureName = featureName;
            this.Dependencies = dependencies ?? new string[0];
        }

        /// <summary>
        /// Gets the feature.
        /// </summary>
        /// <value>
        /// The feature.
        /// </value>
        public string FeatureName { get; }

        /// <summary>
        /// Gets the feature dependencies.
        /// </summary>
        /// <value>
        /// The dependencies.
        /// </value>
        public string[] Dependencies { get; }
    }
}