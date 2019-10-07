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
        /// Gets the plugin name.
        /// </summary>
        /// <value>
        /// The plugin name.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the version pattern.
        /// </summary>
        /// <value>
        /// The version pattern.
        /// </value>
        string Version { get; }
    }
}
