// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPluginRepository.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IPluginRepository interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins
{
    using Kephas.Application;

    /// <summary>
    /// Interface for plugin repository.
    /// </summary>
    public interface IPluginRepository
    {
        /// <summary>
        /// Gets the plugin information from the store.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="throwOnInvalid">Optional. Indicates whether to throw on invalid plugin data or to return a corrupt-marked data.</param>
        /// <returns>
        /// The plugin data.
        /// </returns>
        PluginData GetPluginData(AppIdentity pluginIdentity, bool throwOnInvalid = true);

        /// <summary>
        /// Stores the plugin data, making it persistable among multiple application runs.
        /// </summary>
        /// <param name="pluginData">Information describing the plugin.</param>
        void StorePluginData(PluginData pluginData);
    }
}
