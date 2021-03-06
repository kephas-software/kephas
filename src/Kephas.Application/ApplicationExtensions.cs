﻿// --------------------------------------------------------------------------------------------------------------------
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
    using Kephas.Commands;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Extension methods for <see cref="IAppRuntime"/>.
    /// </summary>
    public static class ApplicationExtensions
    {
        /// <summary>
        /// The features key.
        /// </summary>
        public const string FeaturesKey = "Features";

        private static readonly IFeatureInfo[] EmptyFeatures = new IFeatureInfo[0];

        /// <summary>
        /// Gets the application features.
        /// </summary>
        /// <param name="appRuntime">The app runtime to act on.</param>
        /// <returns>
        /// The application features.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<IFeatureInfo> GetFeatures(this IAppRuntime appRuntime) => appRuntime?[FeaturesKey] as IEnumerable<IFeatureInfo> ?? EmptyFeatures;

        /// <summary>
        /// Indicates whether the application runtime contains the indicated feature.
        /// </summary>
        /// <remarks>
        /// The name comparison is case insensitive.
        /// </remarks>
        /// <param name="appRuntime">The application runtime to act on.</param>
        /// <param name="featureName">Name of the feature.</param>
        /// <returns>
        /// True if the application runtime contains the indicated feature, false otherwise.
        /// </returns>
        public static bool ContainsFeature(this IAppRuntime? appRuntime, string? featureName)
        {
            if (appRuntime == null || featureName == null)
            {
                return false;
            }

            return appRuntime.GetFeatures().Any(f => string.Equals(f.Name, featureName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Register the application arguments as <see cref="IAppArgs"/> service.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="args">Optional. The application arguments. If not provided, they are retrieved from the command line arguments, if not already registered.</param>
        /// <returns>The provided ambient services.</returns>
        public static IAmbientServices RegisterAppArgs(this IAmbientServices ambientServices, IAppArgs? args = null)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            // register the app args if not already registered or the raw args are provided
            if (args != null)
            {
                ambientServices.Register<IAppArgs>(b => b.WithInstance(args));
            }
            else if (!ambientServices.IsRegistered(typeof(IAppArgs)))
            {
                ambientServices.Register<IAppArgs>(b => b.WithInstance(new AppArgs()));
            }

            return ambientServices;
        }

        /// <summary>
        /// Register the application arguments as <see cref="IAppArgs"/> service.
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
