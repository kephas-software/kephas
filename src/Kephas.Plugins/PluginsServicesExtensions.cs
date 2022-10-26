// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginsServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ambient services plugins extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Plugins.Application;
    using Kephas.Services.Builder;

    /// <summary>
    /// The plugins ambient services builder extensions.
    /// </summary>
    public static class PluginsServicesExtensions
    {
        /// <summary>
        /// Sets the plugins-enabled application runtime to the ambient services.
        /// </summary>
        /// <param name="servicesBuilder">The ambient services.</param>
        /// <param name="appFolder">Optional. The application location. If not specified, the assembly
        ///                           location is used.</param>
        /// <param name="configFolders">Optional. The configuration folders.</param>
        /// <param name="licenseFolders">Optional. The license folders.</param>
        /// <param name="isRoot">Indicates whether the application instance is the root.</param>
        /// <param name="appId">Optional. Identifier for the application.</param>
        /// <param name="appInstanceId">Optional. Identifier for the application instance.</param>
        /// <param name="appVersion">Optional. The application version.</param>
        /// <param name="appArgs">Optional. the application arguments.</param>
        /// <param name="enablePlugins">Optional. True to enable, false to disable the plugins.</param>
        /// <param name="pluginsFolder">Optional. Pathname of the plugins folder.</param>
        /// <param name="config">Optional. The application runtime configuration callback.</param>
        /// <returns>
        /// The provided ambient services.
        /// </returns>
        public static IAppServiceCollectionBuilder WithPluginsAppRuntime(
            this IAppServiceCollectionBuilder servicesBuilder,
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
            Action<PluginsAppRuntime>? config = null)
        {
            servicesBuilder = servicesBuilder ?? throw new ArgumentNullException(nameof(servicesBuilder));

            var appRuntime = new PluginsAppRuntime(
                name => servicesBuilder.AmbientServices.GetServiceInstance<ILogManager>().GetLogger(name),
                null,
                appFolder,
                configFolders,
                licenseFolders,
                isRoot,
                appId,
                appInstanceId,
                appVersion,
                appArgs,
                enablePlugins,
                pluginsFolder);
            config?.Invoke(appRuntime);
            return servicesBuilder.WithAppRuntime(appRuntime);
        }
    }
}
