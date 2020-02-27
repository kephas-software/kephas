// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesPluginsExtensions.cs" company="Kephas Software SRL">
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

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Licensing;
    using Kephas.Plugins.Application;

    /// <summary>
    /// The plugins ambient services builder extensions.
    /// </summary>
    public static class AmbientServicesPluginsExtensions
    {
        /// <summary>
        /// Sets the plugins-enabled application runtime to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="assemblyFilter">Optional. A filter specifying the assembly.</param>
        /// <param name="appFolder">Optional. The application location. If not specified, the assembly
        ///                           location is used.</param>
        /// <param name="configFolders">Optional. The configuration folders.</param>
        /// <param name="licenseFolders">Optional. The license folders.</param>
        /// <param name="appId">Optional. Identifier for the application.</param>
        /// <param name="appInstanceId">Optional. Identifier for the application instance.</param>
        /// <param name="appVersion">Optional. The application version.</param>
        /// <param name="appArgs">Optional. the application arguments.</param>
        /// <param name="enablePlugins">Optional. True to enable, false to disable the plugins.</param>
        /// <param name="pluginsFolder">Optional. Pathname of the plugins folder.</param>
        /// <param name="targetFramework">Optional. The target framework.</param>
        /// <returns>
        /// The provided ambient services.
        /// </returns>
        public static IAmbientServices WithPluginsAppRuntime(
            this IAmbientServices ambientServices,
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
            string targetFramework = null)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            return ambientServices.WithAppRuntime(
                new PluginsAppRuntime(
                    (appid, ctx) => ambientServices.LicensingManager.CheckLicense(appid, ctx),
                    ambientServices.LogManager,
                    assemblyFilter,
                    appFolder,
                    configFolders,
                    licenseFolders,
                    appId,
                    appInstanceId,
                    appVersion,
                    appArgs,
                    enablePlugins,
                    pluginsFolder,
                    targetFramework));
        }
    }
}
