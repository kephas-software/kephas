// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginsAppRuntime.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the plugins application runtime class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

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
    using Kephas.Services;

    /// <summary>
    /// The Plugins application runtime.
    /// </summary>
    public class PluginsAppRuntime : DynamicAppRuntime
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
        public const string DefaultPluginsFolder = "Plugins";

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginsAppRuntime"/> class.
        /// </summary>
        /// <param name="assemblyLoader">Optional. The assembly loader.</param>
        /// <param name="checkLicense">Optional. The check license delegate.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        /// <param name="assemblyFilter">Optional. A filter for loaded assemblies.</param>
        /// <param name="appFolder">Optional. The application location. If not specified, the current
        ///                           application location is considered.</param>
        /// <param name="configFolders">Optional. The configuration folders.</param>
        /// <param name="licenseFolders">Optional. The license folders.</param>
        /// <param name="appId">Optional. Identifier for the application.</param>
        /// <param name="appInstanceId">Optional. Identifier for the application instance.</param>
        /// <param name="appVersion">Optional. The application version.</param>
        /// <param name="appArgs">Optional. The application arguments.</param>
        /// <param name="enablePlugins">Optional. True to enable, false to disable the plugins.</param>
        /// <param name="pluginsFolder">Optional. Pathname of the plugins folder.</param>
        /// <param name="targetFramework">Optional. The target framework.</param>
        /// <param name="pluginRepository">Optional. The plugin repository.</param>
        public PluginsAppRuntime(
            IAssemblyLoader assemblyLoader = null,
            Func<AppIdentity, IContext?, ILicenseCheckResult>? checkLicense = null,
            ILogManager logManager = null,
            Func<AssemblyName, bool> assemblyFilter = null,
            string appFolder = null,
            IEnumerable<string> configFolders = null,
            IEnumerable<string> licenseFolders = null,
            string appId = null,
            string appInstanceId = null,
            string appVersion = null,
            IExpando appArgs = null,
            bool? enablePlugins = null,
            string pluginsFolder = null,
            string targetFramework = null,
            IPluginRepository pluginRepository = null)
            : base(assemblyLoader, checkLicense, logManager, assemblyFilter, appFolder, configFolders, licenseFolders, appId, appInstanceId, appVersion, appArgs)
        {
            this.EnablePlugins = this.ComputeEnablePlugins(enablePlugins, appArgs);
            this.PluginsLocation = this.ComputePluginsLocation(pluginsFolder, appArgs);
            this.TargetFramework = this.ComputeTargetFramework(targetFramework, appArgs);
            this.PluginRepository = pluginRepository ?? new PluginRepository(appIdentity => this.GetAppLocation(appIdentity, throwOnNotFound: false));
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
        /// Gets the plugin repository.
        /// </summary>
        /// <value>
        /// The plugin repository.
        /// </value>
        protected internal IPluginRepository PluginRepository { get; }

        /// <summary>
        /// Gets the location of the application with the indicated identity.
        /// </summary>
        /// <param name="appIdentity">The application identity.</param>
        /// <param name="throwOnNotFound">Optional. True to throw if the indicated app is not found.</param>
        /// <returns>
        /// A path indicating the indicated application location.
        /// </returns>
        public override string GetAppLocation(AppIdentity appIdentity, bool throwOnNotFound = true)
        {
            var location = base.GetAppLocation(appIdentity, throwOnNotFound: false);
            if (location != null)
            {
                return location;
            }

            var targetFramework = this.TargetFramework;
            var primaryLocation = Path.Combine(this.PluginsLocation, appIdentity.Id);

            if (!string.IsNullOrEmpty(targetFramework))
            {
                var frameworkSpecificLocation = Path.Combine(primaryLocation, targetFramework);
                if (Directory.Exists(frameworkSpecificLocation))
                {
                    return frameworkSpecificLocation;
                }
            }

            if (Directory.Exists(primaryLocation))
            {
                return primaryLocation;
            }

            return throwOnNotFound
                ? throw new InvalidOperationException($"App '{appIdentity}' not found.")
                : (string)null;
        }

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
                var pluginsDirectories = this.GetPluginsInstallationLocations();
                appDirectories.AddRange(pluginsDirectories.Where(this.CanLoadPlugin));
            }

            this.Logger.Info(Strings.PluginsAppRuntime_LoadingApplicationFolders_Message, appDirectories, this.EnablePlugins ? "enabled" : "disabled");

            return appDirectories;
        }

        /// <summary>
        /// Gets the installed plugins.
        /// </summary>
        /// <returns>
        /// The installed plugins.
        /// </returns>
        public virtual IEnumerable<PluginData> GetInstalledPlugins()
        {
            if (Directory.Exists(this.PluginsLocation))
            {
                var pluginsDirectories = Directory.EnumerateDirectories(this.PluginsLocation);

                foreach (var pluginDirectory in pluginsDirectories)
                {
                    var pluginId = Path.GetFileName(pluginDirectory);
                    yield return this.PluginRepository.GetPluginData(new AppIdentity(pluginId));
                }
            }
        }

        /// <summary>
        /// Gets the installation locations for plugins.
        /// </summary>
        /// <returns>
        /// The installation locations for plugins.
        /// </returns>
        public virtual IEnumerable<string> GetPluginsInstallationLocations()
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
            return enablePlugins ?? (bool?)appArgs?[EnablePluginsArgName] ?? true;
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
            var pluginsFolder = Path.Combine(this.GetAppLocation(), rawPluginsFolder ?? appArgs?[PluginsFolderArgName] as string ?? DefaultPluginsFolder);
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
            return targetFramework ?? appArgs?[TargetFrameworkArgName] as string;
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
            var pluginIdentity = new AppIdentity(pluginId);
            var pluginData = this.PluginRepository.GetPluginData(pluginIdentity);
            pluginIdentity = pluginData.Identity;

            var shouldLoadPlugin = pluginData.State == PluginState.PendingInitialization || pluginData.State == PluginState.Enabled;
            if (shouldLoadPlugin)
            {
                try
                {
                    var licenseState = this.LicensingManager.CheckLicense(pluginIdentity);
                    if (!licenseState.IsLicensed)
                    {
                        this.Logger.Warn("Plugin '{plugin}' is not licensed, will not be loaded. Checker information: {messages}.", pluginIdentity, licenseState.Messages.Select(m => m.Message).ToArray());
                    }

                    return licenseState.IsLicensed;
                }
                catch (Exception ex)
                {
                    this.Logger.Error(ex, "Error while checking the license for plugin {plugin}.", pluginIdentity);
                    return false;
                }
            }

            return false;
        }
    }
}
