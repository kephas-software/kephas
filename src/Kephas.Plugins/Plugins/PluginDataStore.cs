// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginDataStore.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the plugin data store class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins
{
    using System;
    using System.IO;

    using Kephas.Application;

    /// <summary>
    /// A plugin data store.
    /// </summary>
    internal class PluginDataStore : IPluginDataStore
    {
        private const string PluginDataFileName = ".plugindata";

        private readonly Func<AppIdentity, string> pluginLocationResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginDataStore"/> class.
        /// </summary>
        /// <param name="pluginLocationResolver">The plugin location resolver.</param>
        public PluginDataStore(Func<AppIdentity, string> pluginLocationResolver)
        {
            this.pluginLocationResolver = pluginLocationResolver;
        }

        /// <summary>
        /// Gets the installed plugin state and version.
        /// </summary>
        /// <param name="appIdentity">The application identity.</param>
        /// <returns>
        /// The plugin state and version.
        /// </returns>
        public PluginData GetPluginData(AppIdentity appIdentity)
        {
            var pluginLocation = this.pluginLocationResolver(appIdentity);
            if (string.IsNullOrEmpty(pluginLocation))
            {
                return new PluginData(appIdentity, PluginState.None);
            }

            var pluginDataFile = Path.Combine(pluginLocation, PluginDataFileName);
            if (!File.Exists(pluginDataFile))
            {
                return new PluginData(appIdentity, PluginState.None);
            }

            var pluginDataString = File.ReadAllText(pluginDataFile);
            var pluginData = PluginData.Parse(pluginDataString);
            if (!appIdentity.IsMatch(pluginData.Identity))
            {
                throw new InvalidPluginDataException($"Identity mismatch for stored plugin data: '{appIdentity}' requested, but '{pluginData.Identity}' found.");
            }

            return pluginData;
        }

        /// <summary>
        /// Stores the plugin data, making it persistable among multiple application runs.
        /// </summary>
        /// <param name="pluginData">Information describing the plugin.</param>
        public void StorePluginData(PluginData pluginData)
        {
            var pluginLocation = this.pluginLocationResolver(pluginData.Identity);
            if (string.IsNullOrEmpty(pluginLocation))
            {
                throw new InvalidOperationException($"A plugin location could not be resolved for '{pluginData.Identity}'");
            }

            var pluginDataFile = Path.Combine(pluginLocation, PluginDataFileName);
            if (pluginData.State == PluginState.None)
            {
                if (File.Exists(pluginDataFile))
                {
                    File.Delete(pluginDataFile);
                }

                return;
            }

            File.WriteAllText(pluginDataFile, pluginData.ToString());
        }
    }
}
