// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application runtime extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Kephas.Application.Reflection;
    using Kephas.Services.Builder;

    /// <summary>
    /// Extension methods for <see cref="IAppRuntime"/>.
    /// </summary>
    public static class ApplicationServicesExtensions
    {
        /// <summary>
        /// Add the application arguments as <see cref="IAppArgs"/> service.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="args">Optional. The application arguments. If not provided, they are retrieved from the command line arguments, if not already registered.</param>
        /// <returns>The provided ambient services.</returns>
        public static IAmbientServices AddAppArgs(this IAmbientServices ambientServices, IAppArgs? args = null)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            // register the app args if not already registered or the raw args are provided
            if (args != null)
            {
                ambientServices.Add<IAppArgs>(args);
            }
            else if (!ambientServices.Contains(typeof(IAppArgs)))
            {
                ambientServices.Add<IAppArgs>(new AppArgs());
            }

            return ambientServices;
        }

        /// <summary>
        /// Add the application arguments as <see cref="IAppArgs"/> service.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="args">Optional. The application arguments. If not provided, they are retrieved from the command line arguments.</param>
        /// <returns>The provided ambient services.</returns>
        public static IAmbientServices AddAppArgs(this IAmbientServices ambientServices, IEnumerable<string>? args = null) =>
            AddAppArgs(ambientServices, args == null ? null : new AppArgs(args));

        /// <summary>
        /// Add the application arguments as <see cref="IAppArgs"/> service.
        /// </summary>
        /// <param name="servicesBuilder">The ambient services.</param>
        /// <param name="args">Optional. The application arguments. If not provided, they are retrieved from the command line arguments, if not already registered.</param>
        /// <returns>The provided ambient services.</returns>
        public static IAppServiceCollectionBuilder AddAppArgs(this IAppServiceCollectionBuilder servicesBuilder, IAppArgs? args = null)
        {
            servicesBuilder = servicesBuilder ?? throw new ArgumentNullException(nameof(servicesBuilder));

            AddAppArgs(servicesBuilder.AmbientServices, args);

            return servicesBuilder;
        }

        /// <summary>
        /// Add the application arguments as <see cref="IAppArgs"/> service.
        /// </summary>
        /// <param name="servicesBuilder">The services builder.</param>
        /// <param name="args">Optional. The application arguments. If not provided, they are retrieved from the command line arguments.</param>
        /// <returns>The provided ambient services.</returns>
        public static IAppServiceCollectionBuilder AddAppArgs(this IAppServiceCollectionBuilder servicesBuilder, IEnumerable<string>? args = null) =>
            AddAppArgs(servicesBuilder, args == null ? null : new AppArgs(args));
    }
}
