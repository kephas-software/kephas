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
    using System;
    using System.Collections.Generic;

    using Kephas.Reflection;

    /// <summary>
    /// Interface for plugin information.
    /// </summary>
    public interface IPluginInfo : IElementInfo
    {
        /// <summary>
        /// Gets the plugin description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        string Description { get; }

        /// <summary>
        /// Gets the application version.
        /// </summary>
        /// <value>
        /// The application version.
        /// </value>
        Version Version { get; }

        /// <summary>
        /// Gets the plugin dependencies.
        /// </summary>
        /// <value>
        /// The dependencies.
        /// </value>
        IEnumerable<IPluginDependency> Dependencies { get; }
    }
}
