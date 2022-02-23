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
    using Kephas.Cryptography;
    using Kephas.IO;
    using Kephas.Licensing;
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
                ambientServices.Register(appRuntime);
            }

            return ambientServices;
        }

        /// <summary>
        /// Gets the licensing manager.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// The licensing manager.
        /// </returns>
        public static ILicensingManager GetLicensingManager(this IAmbientServices ambientServices) =>
            (ambientServices ?? throw new ArgumentNullException(nameof(ambientServices))).GetRequiredService<ILicensingManager>();

        /// <summary>
        /// Gets the locations manager.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>The locations manager.</returns>
        public static ILocationsManager GetLocationsManager(this IAmbientServices ambientServices) =>
            (ambientServices ?? throw new ArgumentNullException(nameof(ambientServices))).GetRequiredService<ILocationsManager>();

        /// <summary>
        /// Adds the dynamic application runtime to the ambient services.
        /// </summary>
        /// <remarks>
        /// It uses the <see cref="ITypeLoader"/> and <see cref="ILogManager"/> services from the
        /// ambient services to configure the application runtime. Make sure that these services are
        /// properly configured before using this method.
        /// </remarks>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="assemblyFilter">Optional. A filter specifying the assembly.</param>
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
            Func<AssemblyName, bool>? assemblyFilter = null,
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
                name => ambientServices.LogManager.GetLogger(name),
                (appid, ctx) => ambientServices.GetLicensingManager().CheckLicense(appid, ctx),
                null,
                assemblyFilter,
                appFolder,
                configFolders,
                licenseFolders,
                isRoot,
                appId,
                appInstanceId,
                appVersion,
                getLocations: (name, basePath, relativePaths) => ambientServices.GetLocationsManager().GetLocations(relativePaths, basePath, name));
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
        /// <param name="assemblyFilter">Optional. A filter specifying the assembly.</param>
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
            Func<AssemblyName, bool>? assemblyFilter = null,
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
                name => ambientServices.LogManager.GetLogger(name),
                (appid, ctx) => ambientServices.GetLicensingManager().CheckLicense(appid, ctx),
                null,
                assemblyFilter,
                appFolder,
                configFolders,
                licenseFolders,
                isRoot,
                appId,
                appInstanceId,
                appVersion,
                getLocations: (name, basePath, relativePaths) => ambientServices.GetLocationsManager().GetLocations(relativePaths, basePath, name));
            config?.Invoke(appRuntime);
            return ambientServices.WithAppRuntime(appRuntime);
        }

        /// <summary>
        /// Sets the licensing manager to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="licensingManager">The licensing manager.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices WithLicensingManager(this IAmbientServices ambientServices, ILicensingManager licensingManager)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            licensingManager = licensingManager ?? throw new ArgumentNullException(nameof(licensingManager));

            ambientServices.Register(licensingManager);

            return ambientServices;
        }

        /// <summary>
        /// Sets the default licensing manager to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="encryptionService">The encryption service.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices WithDefaultLicensingManager(this IAmbientServices ambientServices, IEncryptionService encryptionService)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));

            const string LicenseRepositoryKey = "__LicenseRepository";
            ambientServices.Register<ILicensingManager>(new DefaultLicensingManager(appid =>
                ((ambientServices[LicenseRepositoryKey] as ILicenseRepository)
                 ?? (ILicenseRepository)(ambientServices[LicenseRepositoryKey] = new LicenseRepository(ambientServices.GetAppRuntime()!, encryptionService)))
                .GetLicenseData(appid)));

            return ambientServices;
        }

        /// <summary>
        /// Gets the application runtime.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// The application runtime.
        /// </returns>
        public static IAppRuntime? GetAppRuntime(this IAmbientServices ambientServices) =>
            (ambientServices ?? throw new ArgumentNullException(nameof(ambientServices))).GetService<IAppRuntime>();
    }
}