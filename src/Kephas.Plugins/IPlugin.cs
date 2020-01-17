// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPlugin.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IPlugin interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins
{
    using Kephas.Data;
    using Kephas.Dynamic;
    using Kephas.Plugins.Reflection;

    /// <summary>
    /// Interface for plugin.
    /// </summary>
    public interface IPlugin : IExpando, IInstance<IPluginInfo>, IIdentifiable
    {
        /// <summary>
        /// Gets the full pathname of the installation folder.
        /// </summary>
        /// <value>
        /// The full pathname of the installation folder.
        /// </value>
        string FolderPath { get; }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        PluginState State { get; }

        /// <summary>
        /// Gets the identity.
        /// </summary>
        /// <returns>
        /// The identity.
        /// </returns>
        PluginIdentity GetIdentity();
    }
}
