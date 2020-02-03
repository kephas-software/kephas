// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPluginDependency.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IPluginDependency interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Reflection
{
    /// <summary>
    /// Interface for plugin dependency.
    /// </summary>
    public interface IPluginDependency
    {
        /// <summary>
        /// Gets the name of the referenced plugin.
        /// </summary>
        /// <value>
        /// The name of the referenced plugin.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the version range.
        /// </summary>
        /// <value>
        /// The version range.
        /// </value>
        string VersionRange { get; }
    }
}
