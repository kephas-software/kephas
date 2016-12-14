// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Net45AmbientServicesBuilderExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for the AmbientServicesBuilder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Platform.Net45
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    using Kephas.Platform.Net;

    /// <summary>
    /// Extension methods for the <see cref="AmbientServicesBuilder"/>.
    /// </summary>
    public static class Net45AmbientServicesBuilderExtensions
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
        [Obsolete("Use instead the WithNetAppRuntime() extension method.")]
        public static AmbientServicesBuilder WithNet45AppEnvironment(this AmbientServicesBuilder ambientServicesBuilder, Func<AssemblyName, bool> assemblyFilter = null, string appLocation = null)
        {
            Contract.Requires(ambientServicesBuilder != null);

            return NetAmbientServicesBuilderExtensions.WithNetAppRuntime(ambientServicesBuilder, assemblyFilter, appLocation);
        }
    }
}