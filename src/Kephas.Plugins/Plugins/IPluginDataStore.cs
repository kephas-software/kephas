// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPluginDataStore.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IPluginDataProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins
{
    using Kephas.Application;

    /// <summary>
    /// Interface for plugin data store.
    /// </summary>
    public interface IPluginDataStore
    {
        /// <summary>
        /// Gets the plugin information from the store.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <returns>
        /// The plugin data.
        /// </returns>
        PluginData GetPluginData(AppIdentity pluginIdentity);

        /// <summary>
        /// Stores the plugin data, making it persistable among multiple application runs.
        /// </summary>
        /// <param name="pluginData">Information describing the plugin.</param>
        void StorePluginData(PluginData pluginData);
    }
}
