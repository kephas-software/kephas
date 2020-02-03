// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginDependency.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the plugin dependency class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Reflection
{
    /// <summary>
    /// A plugin dependency.
    /// </summary>
    public class PluginDependency : IPluginDependency
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginDependency"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="versionRange">The version range.</param>
        public PluginDependency(string name, string versionRange = null)
        {
            this.Name = name;
            this.VersionRange = versionRange;
        }

        /// <summary>
        /// Gets the name of the referenced plugin.
        /// </summary>
        /// <value>
        /// The name of the referenced plugin.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the version range.
        /// </summary>
        /// <value>
        /// The version range.
        /// </value>
        public string VersionRange { get; }
    }
}
