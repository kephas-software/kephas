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
    using System.Collections.Generic;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Reflection;

    /// <summary>
    /// Provides information about an application feature.
    /// </summary>
    public class FeatureInfo : Expando, IElementInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureInfo"/> class.
        /// </summary>
        /// <param name="name">The feature name.</param>
        /// <param name="dependencies">The feature dependencies (optional).</param>
        public FeatureInfo(string name, string[] dependencies = null)
        {
            Requires.NotNullOrEmpty(name, nameof(name));

            this.Name = name;
            this.Dependencies = dependencies ?? new string[0];
        }

        /// <summary>
        /// Gets the feature.
        /// </summary>
        /// <value>
        /// The feature.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the feature dependencies.
        /// </summary>
        /// <value>
        /// The dependencies.
        /// </value>
        public string[] Dependencies { get; }

        /// <summary>
        /// Full name of the <see cref="FeatureInfo"/>.
        /// </summary>
        string IElementInfo.FullName => this.Name;

        /// <summary>
        /// Annotations of the <see cref="FeatureInfo"/>.
        /// </summary>
        IEnumerable<object> IElementInfo.Annotations => null;

        /// <summary>
        /// Declaring container of the <see cref="FeatureInfo"/>.
        /// </summary>
        IElementInfo IElementInfo.DeclaringContainer => null;
    }
}