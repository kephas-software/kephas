// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionAmbientServicesBuilderExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the net ambient services builder extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Extension methods for the <see cref="AmbientServicesBuilder"/>.
    /// </summary>
    public static class ReflectionAmbientServicesBuilderExtensions
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
        public static AmbientServicesBuilder WithDefaultAppRuntime(this AmbientServicesBuilder ambientServicesBuilder, Func<AssemblyName, bool> assemblyFilter = null, string appLocation = null)
        {
            Requires.NotNull(ambientServicesBuilder, nameof(ambientServicesBuilder));

            var ambientServices = ambientServicesBuilder.AmbientServices;
            return ambientServicesBuilder.WithAppRuntime(new DefaultAppRuntime(ambientServices.AssemblyLoader, ambientServices.LogManager, assemblyFilter, appLocation));
        }
    }
}