// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetStandardAmbientServicesBuilderExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for the AmbientServicesBuilder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Platform.NetStandard
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using Kephas.Application;
    using Kephas.Reflection;

    /// <summary>
    /// Extension methods for the <see cref="AmbientServicesBuilder"/>.
    /// </summary>
    public static class NetStandardAmbientServicesBuilderExtensions
    {
        /// <summary>
        /// Sets the .NET Standard 1.4 application runtime to the ambient services.
        /// </summary>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <param name="assemblyFilter">A filter specifying the assembly (optional).</param>
        /// <param name="appLocation">The application location (optional).</param>
        /// <returns>
        /// The provided ambient services builder.
        /// </returns>
        public static AmbientServicesBuilder WithNetStandardAppRuntime(this AmbientServicesBuilder ambientServicesBuilder, Func<AssemblyName, bool> assemblyFilter = null, string appLocation = null)
        {
            Contract.Requires(ambientServicesBuilder != null);

            var assemblyLoader = new NetStandardAssemblyLoader();
            ambientServicesBuilder.AmbientServices.RegisterService<IAssemblyLoader>(assemblyLoader);
            return ambientServicesBuilder.WithAppRuntime(new NetStandardAppRuntime(assemblyLoader, assemblyFilter, appLocation));
        }
    }
}