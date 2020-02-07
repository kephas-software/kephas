// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppRuntimeExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application runtime extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Application;
    using Kephas.Plugins;
    using Kephas.Plugins.Application;

    /// <summary>
    /// An application runtime extensions.
    /// </summary>
    public static class AppRuntimeExtensions
    {
        /// <summary>
        /// Gets the folder where the plugins are installed.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <returns>
        /// The plugins location.
        /// </returns>
        public static string GetPluginsLocation(this IAppRuntime appRuntime)
        {
            if (appRuntime is PluginsAppRuntime pluginsAppRuntime)
            {
                return pluginsAppRuntime.PluginsLocation;
            }

            return appRuntime?[nameof(PluginsAppRuntime.PluginsLocation)] as string;
        }

        /// <summary>
        /// Gets the folder where the plugins are installed.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <returns>
        /// The plugins location.
        /// </returns>
        public static IEnumerable<string> GetPluginsInstallationLocations(this IAppRuntime appRuntime)
        {
            if (appRuntime is PluginsAppRuntime pluginsAppRuntime)
            {
                return pluginsAppRuntime.GetPluginsInstallationLocations();
            }

#if NET45
            return new string[0];
#else
            return Array.Empty<string>();
#endif
        }

        /// <summary>
        /// Gets the installed plugins.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <returns>
        /// The installed plugins.
        /// </returns>
        public static IEnumerable<PluginData> GetInstalledPlugins(this IAppRuntime appRuntime)
        {
            if (appRuntime is PluginsAppRuntime pluginsAppRuntime)
            {
                return pluginsAppRuntime.GetInstalledPlugins();
            }

#if NET45
            return new PluginData[0];
#else
            return Enumerable.Empty<PluginData>();
#endif
        }

        /// <summary>
        /// Gets the application's target framework.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <returns>
        /// The target framework.
        /// </returns>
        public static string GetTargetFramework(this IAppRuntime appRuntime)
        {
            if (appRuntime is PluginsAppRuntime pluginsAppRuntime)
            {
                return pluginsAppRuntime.TargetFramework;
            }

            return appRuntime?[nameof(PluginsAppRuntime.TargetFramework)] as string;
        }

        /// <summary>
        /// Gets a value indicating whether the plugins are enabled.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <returns>
        /// A value indicating whether the plugins are enabled.
        /// </returns>
        public static bool PluginsEnabled(this IAppRuntime appRuntime)
        {
            if (appRuntime is PluginsAppRuntime pluginsAppRuntime)
            {
                return pluginsAppRuntime.EnablePlugins;
            }

            return (bool?)appRuntime?[nameof(PluginsAppRuntime.EnablePlugins)] ?? false;
        }

        /// <summary>
        /// Gets the plugin data service.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <returns>
        /// The plugin data service.
        /// </returns>
        internal static IPluginDataStore GetPluginDataStore(this IAppRuntime appRuntime)
        {
            if (appRuntime is PluginsAppRuntime pluginsAppRuntime)
            {
                return pluginsAppRuntime.PluginDataStore;
            }

            if (!(appRuntime?[nameof(PluginsAppRuntime.PluginDataStore)] is IPluginDataStore pluginDataStore))
            {
                throw new PluginOperationException($"Cannot get the {nameof(PluginsAppRuntime.PluginDataStore)} from {appRuntime.GetType()}.");
            }

            return pluginDataStore;
        }
    }
}
