// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPluginDataService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IPluginDataProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins
{
    using System;
    using System.IO;

    /// <summary>
    /// Interface for plugin data service.
    /// </summary>
    public interface IPluginDataService
    {
        /// <summary>
        /// Gets the installed plugin state and version.
        /// </summary>
        /// <param name="pluginLocation">Pathname of the plugin bin folder.</param>
        /// <returns>
        /// The plugin state and version.
        /// </returns>
        (PluginState state, string version) GetPluginData(string pluginLocation);

        /// <summary>
        /// Sets the plugin state, writing the state file in the provided bin folder.
        /// </summary>
        /// <param name="pluginLocation">Pathname of the plugin bin folder.</param>
        /// <param name="state">State of the plugin.</param>
        /// <param name="version">The plugin version.</param>
        void SetPluginData(string pluginLocation, PluginState state, string version);
    }

    /// <summary>
    /// A plugin data service.
    /// </summary>
    internal class PluginDataService : IPluginDataService
    {
        /// <summary>
        /// Name of the plugin state file.
        /// </summary>
        public const string PluginStateFileName = ".pluginstate";

        /// <summary>
        /// Gets the installed plugin state and version.
        /// </summary>
        /// <param name="pluginLocation">Pathname of the plugin bin folder.</param>
        /// <returns>
        /// The plugin state and version.
        /// </returns>
        public virtual (PluginState state, string version) GetPluginData(string pluginLocation)
        {
            var pluginStateFile = Path.Combine(pluginLocation, PluginStateFileName);
            if (!File.Exists(pluginStateFile))
            {
                return (PluginState.None, null);
            }

            var pluginDataArray = File.ReadAllText(pluginStateFile)?.Split(new[] { ',' }) ?? new string[0];
            PluginState state;
            if (!(pluginDataArray.Length > 0 && Enum.TryParse(pluginDataArray[0], ignoreCase: true, out state)))
            {
                state = PluginState.Corrupt;
            }

            return (state, pluginDataArray.Length > 1 ? pluginDataArray[1] : null);
        }

        /// <summary>
        /// Sets the plugin state, writing the state file in the provided bin folder.
        /// </summary>
        /// <param name="pluginLocation">Pathname of the plugin bin folder.</param>
        /// <param name="state">State of the plugin.</param>
        /// <param name="version">The plugin version.</param>
        public virtual void SetPluginData(string pluginLocation, PluginState state, string version)
        {
            var pluginStateFile = Path.Combine(pluginLocation, PluginStateFileName);
            if (state == PluginState.None)
            {
                if (File.Exists(pluginStateFile))
                {
                    File.Delete(pluginStateFile);
                }

                return;
            }

            File.WriteAllText(pluginStateFile, $"{state},{version}");
        }
    }
}
