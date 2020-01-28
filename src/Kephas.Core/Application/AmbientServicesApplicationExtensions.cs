// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesApplicationExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the net ambient services builder extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Reflection;

    /// <summary>
    /// Extension methods for the <see cref="IAmbientServices"/>.
    /// </summary>
    public static class AmbientServicesApplicationExtensions
    {
        /// <summary>
        /// Adds the dynamic application runtime to the ambient services.
        /// </summary>
        /// <remarks>
        /// It uses the <see cref="IAssemblyLoader"/> and <see cref="ILogManager"/> services from the
        /// ambient services to configure the application runtime. Make sure that these services are
        /// properly configured before using this method.
        /// </remarks>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="assemblyFilter">Optional. A filter specifying the assembly.</param>
        /// <param name="appLocation">Optional. The application location.</param>
        /// <param name="appId">Optional. Identifier for the application.</param>
        /// <param name="appInstanceId">Optional. Identifier for the application instance.</param>
        /// <param name="appVersion">Optional. The application version.</param>
        /// <param name="config">Optional. The application runtime configuration callback.</param>
        /// <returns>
        /// The provided ambient services builder.
        /// </returns>
        public static IAmbientServices WithDynamicAppRuntime(
            this IAmbientServices ambientServices,
            Func<AssemblyName, bool> assemblyFilter = null,
            string appLocation = null,
            string appId = null,
            string appInstanceId = null,
            string appVersion = null,
            Action<DynamicAppRuntime> config = null)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            var appRuntime = new DynamicAppRuntime(ambientServices.AssemblyLoader, ambientServices.LicensingManager, ambientServices.LogManager, assemblyFilter, appLocation, appId, appInstanceId, appVersion);
            config?.Invoke(appRuntime);
            return ambientServices.WithAppRuntime(appRuntime);
        }

        /// <summary>
        /// Adds the static application runtime to the ambient services.
        /// </summary>
        /// <remarks>
        /// It uses the <see cref="IAssemblyLoader"/> and <see cref="ILogManager"/> services from the
        /// ambient services to configure the application runtime. Make sure that these services are
        /// properly configured before using this method.
        /// </remarks>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="assemblyFilter">Optional. A filter specifying the assembly.</param>
        /// <param name="appLocation">Optional. The application location.</param>
        /// <param name="appId">Optional. Identifier for the application.</param>
        /// <param name="appInstanceId">Optional. Identifier for the application instance.</param>
        /// <param name="appVersion">Optional. The application version.</param>
        /// <param name="config">Optional. The application runtime configuration callback.</param>
        /// <returns>
        /// The provided ambient services builder.
        /// </returns>
        public static IAmbientServices WithStaticAppRuntime(
            this IAmbientServices ambientServices,
            Func<AssemblyName, bool> assemblyFilter = null,
            string appLocation = null,
            string appId = null,
            string appInstanceId = null,
            string appVersion = null,
            Action<StaticAppRuntime> config = null)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            var appRuntime = new StaticAppRuntime(ambientServices.AssemblyLoader, ambientServices.LicensingManager, ambientServices.LogManager, assemblyFilter, appLocation, appId, appInstanceId, appVersion);
            config?.Invoke(appRuntime);
            return ambientServices.WithAppRuntime(appRuntime);
        }
    }
}