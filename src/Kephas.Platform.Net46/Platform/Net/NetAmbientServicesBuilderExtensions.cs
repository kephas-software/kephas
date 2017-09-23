// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetAmbientServicesBuilderExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for the AmbientServicesBuilder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Platform.Net
{
    using System;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Configuration;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;

    /// <summary>
    /// Extension methods for the <see cref="AmbientServicesBuilder"/>.
    /// </summary>
    public static class NetAmbientServicesBuilderExtensions
    {
        /// <summary>
        /// Sets the .NET 4.6 application runtime to the ambient services.
        /// </summary>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <param name="assemblyFilter">A filter specifying the assembly (optional).</param>
        /// <param name="appLocation">The application location (optional). If not specified, the assembly location is used.</param>
        /// <returns>
        /// The provided ambient services builder.
        /// </returns>
        public static AmbientServicesBuilder WithNetAppRuntime(this AmbientServicesBuilder ambientServicesBuilder, Func<AssemblyName, bool> assemblyFilter = null, string appLocation = null)
        {
            Requires.NotNull(ambientServicesBuilder, nameof(ambientServicesBuilder));

            var ambientServices = ambientServicesBuilder.AmbientServices;
            var assemblyLoader = new NetAssemblyLoader();
            ambientServices.RegisterService<IAssemblyLoader>(assemblyLoader);

            return ambientServicesBuilder.WithAppRuntime(new NetAppRuntime(assemblyLoader, ambientServices.LogManager, assemblyFilter: assemblyFilter, appLocation: appLocation));
        }

        /// <summary>
        /// Sets the <see cref="AppSettingsConfiguration"/> to the ambient services.
        /// </summary>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <returns>
        /// The provided ambient services builder.
        /// </returns>
        public static AmbientServicesBuilder WithAppSettingsConfiguration(this AmbientServicesBuilder ambientServicesBuilder)
        {
            Requires.NotNull(ambientServicesBuilder, nameof(ambientServicesBuilder));

            var configuration = new AppSettingsConfiguration();
            return ambientServicesBuilder.WithAppConfiguration(configuration);
        }
    }
}