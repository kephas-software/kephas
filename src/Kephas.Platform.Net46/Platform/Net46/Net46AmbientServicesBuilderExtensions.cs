// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Net46AmbientServicesBuilderExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for the AmbientServicesBuilder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Platform.Net46
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Reflection;

    /// <summary>
    /// Extension methods for the <see cref="AmbientServicesBuilder"/>.
    /// </summary>
    public static class Net46AmbientServicesBuilderExtensions
    {
        /// <summary>
        /// Sets the .NET 4.5 application runtime to the ambient services.
        /// </summary>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <param name="assemblyFilter">(Optional) a filter specifying the assembly.</param>
        /// <param name="appLocation">(Optional) the application location.</param>
        /// <returns>
        /// The provided ambient services builder.
        /// </returns>
        public static AmbientServicesBuilder WithNet46AppRuntime(this AmbientServicesBuilder ambientServicesBuilder, Func<AssemblyName, bool> assemblyFilter = null, string appLocation = null)
        {
            Contract.Requires(ambientServicesBuilder != null);

            var assemblyLoader = new Net46AssemblyLoader();
            ambientServicesBuilder.AmbientServices.RegisterService<IAssemblyLoader>(assemblyLoader);
            return ambientServicesBuilder.WithAppRuntime(new Net46AppRuntime(assemblyLoader, assemblyFilter: assemblyFilter, appLocation: appLocation));
        }
    }
}