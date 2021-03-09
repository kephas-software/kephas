// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesAppExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.InteractiveTests.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;

    using Kephas;
    using Kephas.Application;
    using Kephas.Cryptography;
    using Kephas.Diagnostics.Logging;
    using Kephas.Serialization.Json;
    using Newtonsoft.Json;

    public static class AmbientServicesAppExtensions
    {

        /// <summary>
        /// Pre-configures the ambient services asynchronously.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="args">The application arguments.</param>
        /// <param name="encryptionServiceFactory">The encryption service factory.</param>
        /// <param name="debugMode">The debug mode.</param>
        /// <param name="appLifetimeTokenSource">Optional. The application lifetime token source.</param>
        /// <returns>The provided ambient services.</returns>
        public static IAmbientServices PreConfigureAmbientServices(
            this IAmbientServices ambientServices,
            IAppArgs args,
            Func<IAmbientServices, IEncryptionService> encryptionServiceFactory,
            bool debugMode = false,
            CancellationTokenSource? appLifetimeTokenSource = null)
        {
            var rootMode = args.RunAsRoot;

            if (debugMode)
            {
                ambientServices.WithDebugLogManager();
            }
            else
            {
                ambientServices.WithSerilogManager();
            }

            // leave the serilog last, because only so it can take advantage of the
            // assembly resolution from the KisAppRuntime
            ambientServices
                .WithDefaultLicensingManager(encryptionServiceFactory(ambientServices))
                .WithDynamicAppRuntime(
                    assemblyFilter: asm => asm.Name.StartsWith("Kephas") || asm.Name.StartsWith("WebApp"),
                    isRoot: rootMode);

            if (args.LogLevel.HasValue)
            {
                ambientServices.LogManager.MinimumLevel = args.LogLevel.Value;
            }

            return ambientServices;
        }

        /// <summary>
        /// Gets the application assemblies.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>An enumeration of application assemblies.</returns>
        public static IEnumerable<Assembly> GetAppAssemblies(this IAmbientServices ambientServices)
            => ambientServices!.AppRuntime.GetAppAssemblies();

        /// <summary>
        /// Configures the JSON serialization.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="settings">The JSON serialization settings.</param>
        public static void ConfigureJsonSerialization(this IAmbientServices ambientServices, JsonSerializerSettings settings)
        {
            var jsonSettingsProvider = ambientServices.CompositionContainer
                .GetExport<IJsonSerializerSettingsProvider>();
            jsonSettingsProvider.ConfigureJsonSerializerSettings(settings);
            settings.TypeNameHandling = TypeNameHandling.Auto;
        }
    }
}
