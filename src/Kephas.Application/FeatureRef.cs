// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureRef.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the feature reference class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using Kephas.Application.Reflection;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;

    /// <summary>
    /// A feature reference.
    /// </summary>
    public class FeatureRef : Expando
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureRef"/> class.
        /// </summary>
        /// <param name="name">The feature name.</param>
        /// <param name="version">The feature version.</param>
        public FeatureRef(string name, string version)
        {
            Requires.NotNullOrEmpty(name, nameof(name));

            this.Name = name;
            this.Version = string.IsNullOrEmpty(version) ? null : Version.Parse(version);
        }

        /// <summary>
        /// Gets the feature name.
        /// </summary>
        /// <value>
        /// The feature name.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the feature version.
        /// </summary>
        /// <value>
        /// The feature version.
        /// </value>
        public Version Version { get; }

        /// <summary>
        /// Returns a value indicating whether the provided feature is a match for this feature reference.
        /// </summary>
        /// <param name="featureInfo">Information describing the feature.</param>
        /// <returns>
        /// True if match, false if not.
        /// </returns>
        public bool IsMatch(FeatureInfo featureInfo)
        {
            if (featureInfo == null)
            {
                return false;
            }

            if (featureInfo.Name != this.Name)
            {
                return false;
            }

            if (this.Version == null)
            {
                return true;
            }

            return this.Version == featureInfo.Version;
        }
    }
}