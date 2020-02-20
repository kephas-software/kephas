// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppRuntime.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Interface for abstracting away the runtime for the application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;
    using System.IO;
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
        /// Gets the application location (directory where the executing application lies).
        /// </summary>
        /// <returns>
        /// A path indicating the application location.
        /// </returns>
        string GetAppLocation();

        /// <summary>
        /// Gets the location of the application with the indicated identity.
        /// </summary>
        /// <param name="appIdentity">The application identity.</param>
        /// <param name="throwOnNotFound">Optional. True to throw if the indicated app is not found.</param>
        /// <returns>
        /// A path indicating the indicated application location.
        /// </returns>
        string GetAppLocation(AppIdentity? appIdentity, bool throwOnNotFound = true);

        /// <summary>
        /// Gets the application bin directories from where application is loaded.
        /// </summary>
        /// <returns>
        /// The application bin directories.
        /// </returns>
        IEnumerable<string> GetAppBinLocations();

        /// <summary>
        /// Gets the application directories where configuration files are stored.
        /// </summary>
        /// <returns>
        /// The application configuration directories.
        /// </returns>
        IEnumerable<string> GetAppConfigLocations();

        /// <summary>
        /// Gets the application directories where license files are stored.
        /// </summary>
        /// <returns>
        /// The application configuration directories.
        /// </returns>
        IEnumerable<string> GetAppLicenseLocations();

        /// <summary>
        /// Gets the application assemblies.
        /// </summary>
        /// <param name="assemblyFilter">A filter for the assemblies (optional).</param>
        /// <returns>
        /// An enumeration of application assemblies.
        /// </returns>
        IEnumerable<Assembly> GetAppAssemblies(Func<AssemblyName, bool>? assemblyFilter = null);

        /// <summary>
        /// Gets the application's underlying .NET framework identifier.
        /// </summary>
        /// <returns>
        /// The application's underlying .NET framework identifier.
        /// </returns>
        string GetAppFramework();

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
        public static string? GetAppId(this IAppRuntime appRuntime) => appRuntime?[AppRuntimeBase.AppIdKey] as string;

        /// <summary>
        /// Gets the version of the application.
        /// </summary>
        /// <param name="appRuntime">The app runtime to act on.</param>
        /// <returns>
        /// The version of the application.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? GetAppVersion(this IAppRuntime appRuntime) => appRuntime?[AppRuntimeBase.AppVersionKey] as string;

        /// <summary>
        /// Gets the application identity.
        /// </summary>
        /// <param name="appRuntime">The app runtime to act on.</param>
        /// <returns>
        /// The application identity.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AppIdentity? GetAppIdentity(this IAppRuntime appRuntime) => appRuntime?[AppRuntimeBase.AppIdentityKey] as AppIdentity;

        /// <summary>
        /// Gets the identifier of the application instance.
        /// </summary>
        /// <param name="appRuntime">The app runtime to act on.</param>
        /// <returns>
        /// The identifier of the application instance.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? GetAppInstanceId(this IAppRuntime appRuntime) => appRuntime?[AppRuntimeBase.AppInstanceIdKey] as string;

        /// <summary>
        /// Gets the full path of the file or folder name. If the name is a relative path, it will be made relative to the application location.
        /// </summary>
        /// <param name="appRuntime">The app runtime to act on.</param>
        /// <param name="fileName">Name of the file or folder.</param>
        /// <returns>
        /// The full path of the file or folder name.
        /// </returns>
        public static string GetFullPath(this IAppRuntime appRuntime, string fileName)
        {
            return string.IsNullOrEmpty(fileName)
                ? appRuntime.GetAppLocation()
                : Path.GetFullPath(Path.IsPathRooted(fileName) ? fileName : Path.Combine(appRuntime.GetAppLocation(), fileName));
        }
    }
}