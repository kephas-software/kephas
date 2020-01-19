// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppRuntimeExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application runtime extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins
{
    using Kephas.Application;
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
        /// The plugins folder.
        /// </returns>
        public static string GetPluginsFolder(this IAppRuntime appRuntime)
        {
            if (appRuntime is PluginsAppRuntime pluginsAppRuntime)
            {
                return pluginsAppRuntime.PluginsFolder;
            }

            return appRuntime?[nameof(PluginsAppRuntime.PluginsFolder)] as string;
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
    }
}
