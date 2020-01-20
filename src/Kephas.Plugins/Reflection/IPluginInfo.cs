// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPluginInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IPluginInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Reflection
{
    using System.Collections.Generic;

    using Kephas.Reflection;

    /// <summary>
    /// Interface for plugin information.
    /// </summary>
    public interface IPluginInfo : ITypeInfo
    {
        /// <summary>
        /// Gets the tags.
        /// </summary>
        /// <value>
        /// The tags.
        /// </value>
        public string[] Tags { get; }

        /// <summary>
        /// Gets the plugin description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        string Description { get; }

        /// <summary>
        /// Gets the plugin version.
        /// </summary>
        /// <value>
        /// The plugin version.
        /// </value>
        string Version { get; }

        /// <summary>
        /// Gets the plugin dependencies.
        /// </summary>
        /// <value>
        /// The dependencies.
        /// </value>
        IEnumerable<IPluginDependency> Dependencies { get; }

        /// <summary>
        /// Gets the identity.
        /// </summary>
        /// <returns>
        /// The identity.
        /// </returns>
        PluginIdentity GetIdentity();
    }
}
