// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFeatureInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IFeatureInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Reflection
{
    using System;

    using Kephas.Reflection;

    /// <summary>
    /// Contract providing information about an application feature.
    /// </summary>
    public interface IFeatureInfo : IElementInfo
    {
        /// <summary>
        /// Gets the feature dependencies.
        /// </summary>
        /// <value>
        /// The names of features upon which this feature depends.
        /// </value>
        string[] Dependencies { get; }

        /// <summary>
        /// Gets the feature version.
        /// </summary>
        /// <value>
        /// The feature version.
        /// </value>
        Version Version { get; }

        /// <summary>
        /// Gets a value indicating whether this feature is required.
        /// </summary>
        /// <value>
        /// True if this feature is required, false if not.
        /// </value>
        bool IsRequired { get; }
    }
}