// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the plugin helper class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins
{
    using System;
    using System.IO;

    /// <summary>
    /// A plugin helper.
    /// </summary>
    public static class PluginHelper
    {
        /// <summary>
        /// Name of the plugins folder argument.
        /// </summary>
        public const string PluginsFolderArgName = "PluginsFolder";

        /// <summary>
        /// Name of the enable plugins argument.
        /// </summary>
        public const string EnablePluginsArgName = "EnablePlugins";

        /// <summary>
        /// Name of the target framework argument.
        /// </summary>
        public const string TargetFrameworkArgName = "TargetFramework";

        /// <summary>
        /// Pathname of the plugins folder.
        /// </summary>
        public const string PluginsFolder = "Plugins";

        /// <summary>
        /// Name of the plugin state file.
        /// </summary>
        public const string PluginStateFileName = ".pluginstate";

        /// <summary>
        /// Gets the plugin state, reading it from the state file in the provided bin folder.
        /// </summary>
        /// <param name="pluginLocation">Pathname of the plugin bin folder.</param>
        /// <returns>
        /// The plugin state.
        /// </returns>
        public static PluginState GetPluginState(string pluginLocation)
        {
            var pluginStateFile = Path.Combine(pluginLocation, PluginStateFileName);
            if (!File.Exists(pluginStateFile))
            {
                return PluginState.None;
            }

            var stateString = File.ReadAllText(pluginStateFile);
            if (Enum.TryParse<PluginState>(stateString, ignoreCase: true, out var state))
            {
                return state;
            }

            return PluginState.Corrupt;
        }
    }
}
