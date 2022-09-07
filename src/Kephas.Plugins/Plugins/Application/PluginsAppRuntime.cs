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
    using Kephas.IO;
    using Kephas.Logging;
    using Kephas.Plugins.Resources;
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
        /// Pathname of the plugins folder.
        /// </summary>
        public const string DefaultPluginsFolder = "plugins";

        private readonly Lazy<string> lazyPluginsLocation;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginsAppRuntime"/> class.
        /// </summary>
        /// <param name="getLogger">Optional. The get logger delegate.</param>
        /// <param name="appAssemblies">Optional. The application assemblies. If not provided, the loaded assemblies are considered.</param>
        /// <param name="appFolder">Optional. The application location. If not specified, the current
        ///                           application location is considered.</param>
        /// <param name="configFolders">Optional. The configuration folders.</param>
        /// <param name="licenseFolders">Optional. The license folders.</param>
        /// <param name="isRoot">Optional. Indicates whether the application instance is the root.</param>
        /// <param name="appId">Optional. Identifier for the application.</param>
        /// <param name="appInstanceId">Optional. Identifier for the application instance.</param>
        /// <param name="appVersion">Optional. The application version.</param>
        /// <param name="appArgs">Optional. The application arguments.</param>
        /// <param name="enablePlugins">Optional. True to enable, false to disable the plugins.</param>
        /// <param name="pluginsFolder">Optional. Pathname of the plugins folder.</param>
        /// <param name="pluginRepository">Optional. The plugin repository.</param>
        /// <param name="getLocations">Optional. Function for getting application locations.</param>
        public PluginsAppRuntime(
            Func<string, ILogger>? getLogger = null,
            IEnumerable<Assembly>? appAssemblies = null,
            string? appFolder = null,
            IEnumerable<string>? configFolders = null,
            IEnumerable<string>? licenseFolders = null,
            bool? isRoot = null,
            string? appId = null,
            string? appInstanceId = null,
            string? appVersion = null,
            IDynamic? appArgs = null,
            bool? enablePlugins = null,
            string? pluginsFolder = null,
            IPluginRepository? pluginRepository = null,
            Func<string, string, IEnumerable<string>, ILocations>? getLocations = null)
            : base(getLogger, appAssemblies, appFolder, configFolders, licenseFolders, isRoot, appId, appInstanceId, appVersion, appArgs, getLocations)
        {
            this.EnablePlugins = this.ComputeEnablePlugins(enablePlugins, appArgs);
            this.lazyPluginsLocation = new Lazy<string>(() => this.ComputePluginsLocation(pluginsFolder, appArgs));
            this.PluginRepository = pluginRepository ??
                                    new PluginRepository(appIdentity =>
                                        this.GetAppLocation(appIdentity, throwOnNotFound: false));
        }

        /// <summary>
        /// Gets the pathname of the plugins folder.
        /// </summary>
        /// <value>
        /// The pathname of the plugins folder.
        /// </value>
        public string PluginsLocation => this.lazyPluginsLocation.Value;

        /// <summary>
        /// Gets a value indicating whether the plugins are enabled.
        /// </summary>
        /// <value>
        /// True to enable plugins, false to disable them.
        /// </value>
        public bool EnablePlugins { get; }

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
        public override string? GetAppLocation(AppIdentity? appIdentity, bool throwOnNotFound = true)
        {
            var location = base.GetAppLocation(appIdentity, throwOnNotFound: false);
            if (location != null)
            {
                return location;
            }

            var primaryLocation = Path.Combine(this.PluginsLocation, appIdentity!.Id);
            if (Directory.Exists(primaryLocation))
            {
                return primaryLocation;
            }

            return throwOnNotFound
                ? throw new InvalidOperationException($"App '{appIdentity}' not found.")
                : (string?)null;
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
                appDirectories.AddRange(pluginsDirectories.Where(this.CanLoadEmbeddedPluginBinaries));
            }

            this.Logger.Debug(
                Strings.PluginsAppRuntime_LoadingApplicationFolders_Message,
                appDirectories,
                this.EnablePlugins ? "enabled" : "disabled");

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
            return this.GetPluginsInstallationLocations()
                .Select(pluginDirectory => this.PluginRepository.GetPluginData(
                    new AppIdentity(Path.GetFileName(pluginDirectory)),
                    throwOnInvalid: false));
        }

        /// <summary>
        /// Gets the installation locations for plugins.
        /// </summary>
        /// <returns>
        /// The installation locations for plugins.
        /// </returns>
        public virtual IEnumerable<string> GetPluginsInstallationLocations()
        {
            if (!Directory.Exists(this.PluginsLocation))
            {
                return Enumerable.Empty<string>();
            }

            return Directory.EnumerateDirectories(this.PluginsLocation)
                .Where(l => !l.IsHiddenLocation());
        }

        /// <summary>
        /// Calculates a value indicating whether to enable plugins.
        /// </summary>
        /// <param name="enablePlugins">True to enable, false to disable the plugins.</param>
        /// <param name="appArgs">The application arguments.</param>
        /// <returns>
        /// True to enable plugins, false to disable them.
        /// </returns>
        protected virtual bool ComputeEnablePlugins(bool? enablePlugins, IDynamic? appArgs)
        {
            return enablePlugins ?? appArgs?.GetLaxValue(EnablePluginsArgName, true) ?? true;
        }

        /// <summary>
        /// Calculates the plugins folder.
        /// </summary>
        /// <param name="rawPluginsFolder">Pathname of the raw plugins folder.</param>
        /// <param name="appArgs">The application arguments.</param>
        /// <returns>
        /// The calculated plugins folder.
        /// </returns>
        protected virtual string ComputePluginsLocation(string? rawPluginsFolder, IDynamic? appArgs)
        {
            var locations = this.GetLocations(
                DefaultPluginsFolder,
                this.GetAppLocation(),
                new[] { rawPluginsFolder ?? appArgs?[PluginsFolderArgName] as string ?? DefaultPluginsFolder });
            var pluginsFolder = locations.FirstOrDefault();
            if (string.IsNullOrEmpty(pluginsFolder))
            {
                throw new InvalidOperationException("Computing the plugins location yielded an empty value.");
            }

            return Path.GetFullPath(pluginsFolder);
        }

        /// <summary>
        /// Determine if the indicated embedded plugin binaries can be loaded. Load only licensed embedded plugins in
        /// <see cref="PluginState.PendingInitialization"/>, <see cref="PluginState.PendingUninitialization"/>, and <see cref="PluginState.Enabled"/> states.
        /// </summary>
        /// <param name="pluginFolder">Pathname of the plugin folder.</param>
        /// <returns>
        /// True if we can load plugin, false if not.
        /// </returns>
        protected virtual bool CanLoadEmbeddedPluginBinaries(string pluginFolder)
        {
            var pluginId = Path.GetFileName(pluginFolder);
            var pluginIdentity = new AppIdentity(pluginId);
            try
            {
                var pluginData = this.PluginRepository.GetPluginData(pluginIdentity, throwOnInvalid: false);
                pluginIdentity = pluginData.Identity;

                var shouldLoadPlugin = (pluginData.State == PluginState.PendingInitialization
                                        || pluginData.State == PluginState.Enabled
                                        || pluginData.State == PluginState.PendingUninitialization)
                                       && pluginData.Kind == PluginKind.Embedded;
                if (!shouldLoadPlugin)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Error while reading the plugin data for {plugin}.", pluginIdentity);
                return false;
            }

            try
            {
                var licenseState = this.CheckLicense(pluginIdentity, null);
                if (!licenseState.Value)
                {
                    this.Logger.Warn(
                        "Plugin '{plugin}' is not licensed, will not be loaded. Checker information: {messages}.",
                        pluginIdentity,
                        licenseState.Messages.Select(m => m.Message).ToArray());
                }

                return licenseState.Value;
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Error while checking the license for plugin {plugin}.", pluginIdentity);
                return false;
            }
        }

        /// <summary>
        /// Initializes the service, ensuring that the assembly resolution is properly handled.
        /// </summary>
        /// <param name="context">Optional. An optional context for initialization.</param>
        protected override void InitializeCore(IContext? context = null)
        {
            base.InitializeCore(context);

            this.EnsureProbingPath();
        }

        /// <summary>
        /// Ensures that the probing path for assembly loading is properly set.
        /// </summary>
        protected virtual void EnsureProbingPath()
        {
            // TODO check whether this is useful in .NET Standard 2.0+
            // this was a fix for .NET 4.6.1, maybe is useful in future.
        }
    }
}