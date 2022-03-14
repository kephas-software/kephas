// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationExtensions.cs" company="Kephas Software SRL">
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

    /// <summary>
    /// Extension methods for <see cref="IAppRuntime"/>.
    /// </summary>
    public static class ApplicationExtensions
    {
        /// <summary>
        /// RegisterService the application arguments as <see cref="IAppArgs"/> service.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="args">Optional. The application arguments. If not provided, they are retrieved from the command line arguments, if not already registered.</param>
        /// <returns>The provided ambient services.</returns>
        public static IAmbientServices RegisterAppArgs(this IAmbientServices ambientServices, IAppArgs? args = null)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            // register the app args if not already registered or the raw args are provided
            if (args != null)
            {
                ambientServices.Register<IAppArgs>(args);
            }
            else if (!ambientServices.IsRegistered(typeof(IAppArgs)))
            {
                ambientServices.Register<IAppArgs>(new AppArgs());
            }

            return ambientServices;
        }

        /// <summary>
        /// RegisterService the application arguments as <see cref="IAppArgs"/> service.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="args">Optional. The application arguments. If not provided, they are retrieved from the command line arguments.</param>
        /// <returns>The provided ambient services.</returns>
        public static IAmbientServices RegisterAppArgs(this IAmbientServices ambientServices, IEnumerable<string>? args = null)
        {
            return RegisterAppArgs(ambientServices, args == null ? null : new AppArgs(args));
        }
    }
}
