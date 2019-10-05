// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppRuntime.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Interface for abstracting away the runtime for the application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using Kephas.Dynamic;

    /// <summary>
    /// Interface for abstracting away the runtime for the application.
    /// </summary>
    public interface IAppRuntime : IExpando
    {
        /// <summary>
        /// Gets the application location (directory where the application lies).
        /// </summary>
        /// <returns>
        /// A path indicating the application location.
        /// </returns>
        string GetAppLocation();

        /// <summary>
        /// Gets the application bin folders from where application is loaded.
        /// </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the application bin folders in this
        /// collection.
        /// </returns>
        IEnumerable<string> GetAppBinDirectories();

        /// <summary>
        /// Gets the application assemblies.
        /// </summary>
        /// <param name="assemblyFilter">A filter for the assemblies (optional).</param>
        /// <returns>
        /// An enumeration of application assemblies.
        /// </returns>
        IEnumerable<Assembly> GetAppAssemblies(Func<AssemblyName, bool> assemblyFilter = null);

        /// <summary>
        /// Gets host address.
        /// </summary>
        /// <returns>
        /// The host address.
        /// </returns>
        IPAddress GetHostAddress();

        /// <summary>
        /// Gets host name.
        /// </summary>
        /// <returns>
        /// The host name.
        /// </returns>
        string GetHostName();
    }

    /// <summary>
    /// Extension methods for <see cref="IAppRuntime"/>.
    /// </summary>
    public static class AppRuntimeExtensions
    {
        /// <summary>
        /// Gets the identifier of the application.
        /// </summary>
        /// <param name="appRuntime">The app runtime to act on.</param>
        /// <returns>
        /// The identifier of the application.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetAppId(this IAppRuntime appRuntime) => appRuntime?[AppRuntimeBase.AppIdKey] as string;

        /// <summary>
        /// Gets the version of the application.
        /// </summary>
        /// <param name="appRuntime">The app runtime to act on.</param>
        /// <returns>
        /// The version of the application.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetAppVersion(this IAppRuntime appRuntime) => appRuntime?[AppRuntimeBase.AppVersionKey] as string;

        /// <summary>
        /// Gets the identifier of the application instance.
        /// </summary>
        /// <param name="appRuntime">The app runtime to act on.</param>
        /// <returns>
        /// The identifier of the application instance.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetAppInstanceId(this IAppRuntime appRuntime) => appRuntime?[AppRuntimeBase.AppInstanceIdKey] as string;
    }
}