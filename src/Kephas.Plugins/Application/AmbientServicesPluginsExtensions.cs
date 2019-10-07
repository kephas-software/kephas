// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesPluginsExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ambient services plugins extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Application
{
    using System;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;

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
        /// <param name="appLocation">Optional. The application location. If not specified, the
        ///                           assembly location is used.</param>
        /// <param name="appId">Optional. Identifier for the application.</param>
        /// <param name="appVersion">Optional. The application version.</param>
        /// <param name="appArgs">Optional. the application arguments.</param>
        /// <param name="enablePlugins">Optional. True to enable, false to disable the plugins.</param>
        /// <param name="pluginsFolder">Optional. Pathname of the plugins folder.</param>
        /// <param name="targetFramework">Optional. target framework.</param>
        /// <returns>
        /// The provided ambient services.
        /// </returns>
        public static IAmbientServices WithPluginsAppRuntime(
            this IAmbientServices ambientServices,
            Func<AssemblyName, bool> assemblyFilter = null,
            string appLocation = null,
            string appId = null,
            string appVersion = null,
            IExpando appArgs = null,
            bool? enablePlugins = null,
            string pluginsFolder = null,
            string targetFramework = null)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            return ambientServices.WithAppRuntime(
                new PluginsAppRuntime(
                    ambientServices.AssemblyLoader,
                    ambientServices.LogManager,
                    assemblyFilter: assemblyFilter,
                    appLocation: appLocation,
                    appId: appId,
                    appVersion: appVersion,
                    appArgs: appArgs,
                    enablePlugins: enablePlugins,
                    pluginsFolder: pluginsFolder,
                    targetFramework: targetFramework));
        }
    }
}
