// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFeatureInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IFeatureInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
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
    }
}