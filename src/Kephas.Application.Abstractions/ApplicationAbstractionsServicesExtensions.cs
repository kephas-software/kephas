// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationAbstractionsServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using System.Collections.Generic;

    using Kephas.Application;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Services.Builder;

    /// <summary>
    /// Extensions for <see cref="IAmbientServices"/> for applications.
    /// </summary>
    public static class ApplicationAbstractionsServicesExtensions
    {
        /// <summary>
        /// Sets the application runtime to the ambient services.
        /// </summary>
        /// <param name="servicesBuilder">The services builder.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <returns>
        /// The <paramref name="servicesBuilder"/>.
        /// </returns>
        public static IAppServiceCollectionBuilder WithAppRuntime(
            this IAppServiceCollectionBuilder servicesBuilder,
            IAppRuntime appRuntime)
        {
            servicesBuilder = servicesBuilder ?? throw new ArgumentNullException(nameof(servicesBuilder));
            appRuntime = appRuntime ?? throw new ArgumentNullException(nameof(appRuntime));

            var ambientServices = servicesBuilder.AmbientServices;
            var existingAppRuntime = ambientServices.TryGetServiceInstance<IAppRuntime>();
            if (existingAppRuntime != null && existingAppRuntime != appRuntime)
            {
                ServiceHelper.Finalize(existingAppRuntime);
            }

            if (existingAppRuntime != appRuntime)
            {
                ServiceHelper.Initialize(appRuntime);
                if (existingAppRuntime != null)
                {
                    ambientServices.Replace(appRuntime);
                }
                else
                {
                    ambientServices.Add(appRuntime);
                }
            }

            return servicesBuilder;
        }

        /// <summary>
        /// Adds the dynamic application runtime to the ambient services.
        /// </summary>
        /// <remarks>
        /// It uses the <see cref="ITypeLoader"/> and <see cref="ILogManager"/> services from the
        /// ambient services to configure the application runtime. Make sure that these services are
        /// properly configured before using this method.
        /// </remarks>
        /// <param name="servicesBuilder">The services builder.</param>
        /// <param name="appFolder">Optional. The application location.</param>
        /// <param name="configFolders">Optional. The configuration folders.</param>
        /// <param name="licenseFolders">Optional. The license folders.</param>
        /// <param name="isRoot">Optional. Indicates whether the application instance is the root.</param>
        /// <param name="appId">Optional. Identifier for the application.</param>
        /// <param name="appInstanceId">Optional. Identifier for the application instance.</param>
        /// <param name="appVersion">Optional. The application version.</param>
        /// <param name="config">Optional. The application runtime configuration callback.</param>
        /// <param name="settingsConfig">Optional. The settings configuration.</param>
        /// <returns>
        /// The <paramref name="servicesBuilder"/>.
        /// </returns>
        public static IAppServiceCollectionBuilder WithDynamicAppRuntime(
            this IAppServiceCollectionBuilder servicesBuilder,
            Action<AppRuntimeSettings>? settingsConfig = null,
            Action<DynamicAppRuntime>? runtimeConfig = null)
        {
            servicesBuilder = servicesBuilder ?? throw new ArgumentNullException(nameof(servicesBuilder));

            var settings = new AppRuntimeSettings();
            settingsConfig?.Invoke(settings);

            var appRuntime = new DynamicAppRuntime(settings);
            runtimeConfig?.Invoke(appRuntime);

            return servicesBuilder.WithAppRuntime(appRuntime);
        }

        /// <summary>
        /// Adds the static application runtime to the ambient services.
        /// </summary>
        /// <remarks>
        /// It uses the <see cref="ITypeLoader"/> and <see cref="ILogManager"/> services from the
        /// ambient services to configure the application runtime. Make sure that these services are
        /// properly configured before using this method.
        /// </remarks>
        /// <param name="servicesBuilder">The services builder.</param>
        /// <param name="appFolder">Optional. The application location.</param>
        /// <param name="configFolders">Optional. The configuration folders.</param>
        /// <param name="licenseFolders">Optional. The license folders.</param>
        /// <param name="isRoot">Optional. Indicates whether the application instance is the root.</param>
        /// <param name="appId">Optional. Identifier for the application.</param>
        /// <param name="appInstanceId">Optional. Identifier for the application instance.</param>
        /// <param name="appVersion">Optional. The application version.</param>
        /// <param name="config">Optional. The application runtime configuration callback.</param>
        /// <returns>
        /// The provided ambient services.
        /// </returns>
        public static IAppServiceCollectionBuilder WithStaticAppRuntime(
            this IAppServiceCollectionBuilder servicesBuilder,
            string? appFolder = null,
            IEnumerable<string>? configFolders = null,
            IEnumerable<string>? licenseFolders = null,
            bool? isRoot = null,
            string? appId = null,
            string? appInstanceId = null,
            string? appVersion = null,
            Action<StaticAppRuntime>? config = null)
        {
            servicesBuilder = servicesBuilder ?? throw new ArgumentNullException(nameof(servicesBuilder));

            var appRuntime = new StaticAppRuntime(
                name => servicesBuilder.AmbientServices.GetServiceInstance<ILogManager>().GetLogger(name),
                null,
                appFolder,
                configFolders,
                licenseFolders,
                isRoot,
                appId,
                appInstanceId,
                appVersion);
            config?.Invoke(appRuntime);
            return servicesBuilder.WithAppRuntime(appRuntime);
        }
    }
}