﻿// --------------------------------------------------------------------------------------------------------------------
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
        /// Sets the default application runtime to the ambient services.
        /// </summary>
        /// <remarks>
        /// It uses the <see cref="IAssemblyLoader"/> and <see cref="ILogManager"/> services from the
        /// ambient services to configure the application runtime. Make sure that these services are
        /// properly configured before using this method.
        /// </remarks>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="assemblyFilter">A filter specifying the assembly (optional).</param>
        /// <param name="appLocation">The application location (optional).</param>
        /// <returns>
        /// The provided ambient services builder.
        /// </returns>
        public static IAmbientServices WithDefaultAppRuntime(this IAmbientServices ambientServices, Func<AssemblyName, bool> assemblyFilter = null, string appLocation = null)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            return ambientServices.WithAppRuntime(new DefaultAppRuntime(ambientServices.AssemblyLoader, ambientServices.LogManager, assemblyFilter, appLocation));
        }
    }
}