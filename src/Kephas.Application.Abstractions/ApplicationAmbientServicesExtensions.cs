// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationAmbientServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.IO;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Extensions for <see cref="IAmbientServices"/> for applications.
    /// </summary>
    public static class ApplicationAmbientServicesExtensions
    {
        /// <summary>
        /// Sets the application runtime to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices WithAppRuntime(this IAmbientServices ambientServices, IAppRuntime appRuntime)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            appRuntime = appRuntime ?? throw new ArgumentNullException(nameof(appRuntime));

            var existingAppRuntime = ambientServices.GetAppRuntime();
            if (existingAppRuntime != null && existingAppRuntime != appRuntime)
            {
                ServiceHelper.Finalize(existingAppRuntime);
            }

            if (existingAppRuntime != appRuntime)
            {
                ServiceHelper.Initialize(appRuntime);
                ambientServices.Add(appRuntime);
            }

            return ambientServices;
        }

        /// <summary>
        /// Adds the dynamic application runtime to the ambient services.
        /// </summary>
        /// <remarks>
        /// It uses the <see cref="ITypeLoader"/> and <see cref="ILogManager"/> services from the
        /// ambient services to configure the application runtime. Make sure that these services are
        /// properly configured before using this method.
        /// </remarks>
        /// <param name="ambientServices">The ambient services.</param>
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
        public static IAmbientServices WithDynamicAppRuntime(
            this IAmbientServices ambientServices,
            string? appFolder = null,
            IEnumerable<string>? configFolders = null,
            IEnumerable<string>? licenseFolders = null,
            bool? isRoot = null,
            string? appId = null,
            string? appInstanceId = null,
            string? appVersion = null,
            Action<DynamicAppRuntime>? config = null)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            var appRuntime = new DynamicAppRuntime(
                name => ambientServices.GetServiceInstance<ILogManager>().GetLogger(name),
                null,
                appFolder,
                configFolders,
                licenseFolders,
                isRoot,
                appId,
                appInstanceId,
                appVersion);
            config?.Invoke(appRuntime);
            return ambientServices.WithAppRuntime(appRuntime);
        }

        /// <summary>
        /// Adds the static application runtime to the ambient services.
        /// </summary>
        /// <remarks>
        /// It uses the <see cref="ITypeLoader"/> and <see cref="ILogManager"/> services from the
        /// ambient services to configure the application runtime. Make sure that these services are
        /// properly configured before using this method.
        /// </remarks>
        /// <param name="ambientServices">The ambient services.</param>
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
        public static IAmbientServices WithStaticAppRuntime(
            this IAmbientServices ambientServices,
            string? appFolder = null,
            IEnumerable<string>? configFolders = null,
            IEnumerable<string>? licenseFolders = null,
            bool? isRoot = null,
            string? appId = null,
            string? appInstanceId = null,
            string? appVersion = null,
            Action<StaticAppRuntime>? config = null)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            var appRuntime = new StaticAppRuntime(
                name => ambientServices.GetServiceInstance<ILogManager>().GetLogger(name),
                null,
                appFolder,
                configFolders,
                licenseFolders,
                isRoot,
                appId,
                appInstanceId,
                appVersion);
            config?.Invoke(appRuntime);
            return ambientServices.WithAppRuntime(appRuntime);
        }
    }
}