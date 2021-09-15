// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TargetFeatureAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the applies to feature attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;

    using Kephas.Injection.Metadata;

    /// <summary>
    /// Attribute for indicating the feature for which a behavior apply.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TargetFeatureAttribute : Attribute, IMetadataValue<FeatureRef>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetFeatureAttribute"/> class.
        /// </summary>
        /// <param name="featureName">Name of the feature.</param>
        /// <param name="featureVersion">Optional. The feature version.</param>
        public TargetFeatureAttribute(string featureName, string? featureVersion = null)
        {
            this.Value = new FeatureRef(featureName, featureVersion);
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
        public FeatureRef Value { get; }
    }
}