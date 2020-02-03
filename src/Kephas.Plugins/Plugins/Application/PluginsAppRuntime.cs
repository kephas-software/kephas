// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginsAppRuntime.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the plugins application runtime class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Application
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Dynamic;
    using Kephas.Licensing;
    using Kephas.Logging;
    using Kephas.Plugins.Resources;
    using Kephas.Reflection;

    /// <summary>
    /// The Plugins application runtime.
    /// </summary>
    public class PluginsAppRuntime : DynamicAppRuntime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginsAppRuntime"/> class.
        /// </summary>
        /// <param name="assemblyLoader">Optional. The assembly loader.</param>
        /// <param name="licensingManager">Optional. Manager for licensing.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        /// <param name="assemblyFilter">Optional. A filter for loaded assemblies.</param>
        /// <param name="appFolder">Optional. The application location. If not specified, the current
        ///                           application location is considered.</param>
        /// <param name="configFolders">Optional. The configuration folders.</param>
        /// <param name="appId">Optional. Identifier for the application.</param>
        /// <param name="appInstanceId">Optional. Identifier for the application instance.</param>
        /// <param name="appVersion">Optional. The application version.</param>
        /// <param name="appArgs">Optional. The application arguments.</param>
        /// <param name="enablePlugins">Optional. True to enable, false to disable the plugins.</param>
        /// <param name="pluginsFolder">Optional. Pathname of the plugins folder.</param>
        /// <param name="targetFramework">Optional. The target framework.</param>
        public PluginsAppRuntime(
            IAssemblyLoader assemblyLoader = null,
            ILicensingManager licensingManager = null,
            ILogManager logManager = null,
            Func<AssemblyName, bool> assemblyFilter = null,
            string appFolder = null,
            IEnumerable<string> configFolders = null,
            string appId = null,
            string appInstanceId = null,
            string appVersion = null,
            IExpando appArgs = null,
            bool? enablePlugins = null,
            string pluginsFolder = null,
            string targetFramework = null)
            : base(assemblyLoader, licensingManager, logManager, assemblyFilter, appFolder, configFolders, appId, appInstanceId, appVersion, appArgs)
        {
            this.EnablePlugins = this.ComputeEnablePlugins(enablePlugins, appArgs);
            this.PluginsLocation = this.ComputePluginsLocation(pluginsFolder, appArgs);
            this.TargetFramework = this.ComputeTargetFramework(targetFramework, appArgs);
        }

        /// <summary>
        /// Gets the pathname of the plugins folder.
        /// </summary>
        /// <value>
        /// The pathname of the plugins folder.
        /// </value>
        public string PluginsLocation { get; }

        /// <summary>
        /// Gets a value indicating whether the plugins are enabled.
        /// </summary>
        /// <value>
        /// True to enable plugins, false to disable them.
        /// </value>
        public bool EnablePlugins { get; }

        /// <summary>
        /// Gets the target framework.
        /// </summary>
        /// <value>
        /// The target framework.
        /// </value>
        public string TargetFramework { get; }

        /// <summary>
        /// Gets the application bin folders from where application is loaded.
        /// </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the application bin folders in this
        /// collection.
        /// </returns>
        public override IEnumerable<string> GetAppBinLocations()
        {
            var appDirectories = base.GetAppBinLocations().ToList();

            if (this.EnablePlugins)
            {
                var pluginsDirectories = this.GetPluginLocations();
                appDirectories.AddRange(pluginsDirectories.Where(this.CanLoadPlugin));
            }

            this.Logger.Info(Strings.PluginsAppRuntime_LoadingApplicationFolders_Message, appDirectories, this.EnablePlugins ? "enabled" : "disabled");

            return appDirectories;
        }

        /// <summary>
        /// Gets the locations for plugins.
        /// </summary>
        /// <returns>
        /// The locations for plugins.
        /// </returns>
        public virtual IEnumerable<string> GetPluginLocations()
        {
            var targetFramework = this.TargetFramework;

            if (Directory.Exists(this.PluginsLocation))
            {
                var pluginsDirectories = Directory.EnumerateDirectories(this.PluginsLocation);

                foreach (var pluginDirectory in pluginsDirectories)
                {
                    if (string.IsNullOrEmpty(targetFramework))
                    {
                        yield return pluginDirectory;
                    }
                    else
                    {
                        var frameworkSpecificDirectory = Path.Combine(pluginDirectory, targetFramework);
                        yield return Directory.Exists(frameworkSpecificDirectory) ? frameworkSpecificDirectory : pluginDirectory;
                    }
                }
            }
        }

        /// <summary>
        /// Calculates a value indicating whether to enable plugins.
        /// </summary>
        /// <param name="enablePlugins">True to enable, false to disable the plugins.</param>
        /// <param name="appArgs">The application arguments.</param>
        /// <returns>
        /// True to enable plugins, false to disable them.
        /// </returns>
        protected virtual bool ComputeEnablePlugins(bool? enablePlugins, IExpando appArgs)
        {
            return enablePlugins ?? (bool?)appArgs?[PluginHelper.EnablePluginsArgName] ?? true;
        }

        /// <summary>
        /// Calculates the plugins folder.
        /// </summary>
        /// <param name="rawPluginsFolder">Pathname of the raw plugins folder.</param>
        /// <param name="appArgs">The application arguments.</param>
        /// <returns>
        /// The calculated plugins folder.
        /// </returns>
        protected virtual string ComputePluginsLocation(string rawPluginsFolder, IExpando appArgs)
        {
            var pluginsFolder = Path.Combine(this.GetAppLocation(), rawPluginsFolder ?? appArgs?[PluginHelper.PluginsFolderArgName] as string ?? PluginHelper.DefaultPluginsFolder);
            return Path.GetFullPath(pluginsFolder);
        }

        /// <summary>
        /// Calculates the target framework.
        /// </summary>
        /// <param name="targetFramework">The target framework.</param>
        /// <param name="appArgs">The application arguments.</param>
        /// <returns>
        /// The calculated target framework.
        /// </returns>
        protected virtual string ComputeTargetFramework(string targetFramework, IExpando appArgs)
        {
            return targetFramework ?? appArgs?[PluginHelper.TargetFrameworkArgName] as string;
        }

        /// <summary>
        /// Gets the plugin data.
        /// </summary>
        /// <param name="pluginName">Name of the plugin.</param>
        /// <param name="pluginLocation">The plugin location.</param>
        /// <returns>
        /// The plugin data.
        /// </returns>
        protected virtual (PluginState state, string version) GetPluginData(string pluginName, string pluginLocation)
        {
            return PluginHelper.GetPluginData(pluginLocation);
        }

        /// <summary>
        /// Determine if the indicated plugin can be loaded. Load only licensed plugins in
        /// <see cref="PluginState.PendingInitialization"/> and <see cref="PluginState.Enabled"/> states.
        /// </summary>
        /// <param name="pluginFolder">Pathname of the plugin folder.</param>
        /// <returns>
        /// True if we can load plugin, false if not.
        /// </returns>
        protected virtual bool CanLoadPlugin(string pluginFolder)
        {
            var pluginId = Path.GetFileName(pluginFolder);
            var (pluginState, pluginVersion) = this.GetPluginData(Path.GetFileName(pluginFolder), pluginFolder);

            var shouldLoadPlugin = pluginState == PluginState.PendingInitialization || pluginState == PluginState.Enabled;
            if (shouldLoadPlugin)
            {
                var plugin = new AppIdentity(pluginId, pluginVersion);
                try
                {
                    var licenseState = this.LicensingManager.GetLicensingState(plugin);
                    return licenseState.IsLicensed;
                }
                catch (Exception ex)
                {
                    this.Logger.Error(ex, "Error while checking the license for plugin {plugin}.", plugin);
                    return false;
                }
            }

            return false;
        }
    }
}
