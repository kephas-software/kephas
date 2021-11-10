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
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using Kephas.Dynamic;
    using Kephas.IO;

    /// <summary>
    /// Interface for abstracting away the runtime for the application.
    /// </summary>
    public interface IAppRuntime : IExpando
    {
        /// <summary>
        /// The default configuration folder.
        /// </summary>
        public static readonly string DefaultConfigFolder = "config";

        /// <summary>
        /// The default license folder.
        /// </summary>
        public static readonly string DefaultLicenseFolder = "licenses";

        /// <summary>
        /// The application identifier key.
        /// </summary>
        public static readonly string AppIdentityKey = "AppIdentity";

        /// <summary>
        /// The environment name key.
        /// </summary>
        public static readonly string EnvKey = "Env";

        /// <summary>
        /// The application identifier key.
        /// </summary>
        public static readonly string AppIdKey = nameof(IAppArgs.AppId);

        /// <summary>
        /// The application instance identifier key.
        /// </summary>
        public static readonly string AppInstanceIdKey = nameof(IAppArgs.AppInstanceId);

        /// <summary>
        /// The application version key.
        /// </summary>
        public static readonly string AppVersionKey = "AppVersion";

        /// <summary>
        /// The root application identifier key.
        /// </summary>
        public static readonly string IsRootKey = "IsRoot";

        /// <summary>
        /// Gets the application arguments.
        /// </summary>
        IAppArgs AppArgs { get; }

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
        string? GetAppLocation(AppIdentity? appIdentity, bool throwOnNotFound = true);

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
        /// Attempts to load an assembly from its given assembly name.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to be loaded.</param>
        /// <returns>
        /// The resolved assembly reference.
        /// </returns>
        Assembly LoadAssemblyFromName(AssemblyName assemblyName);

        /// <summary>
        /// Attempts to load an assembly.
        /// </summary>
        /// <param name="assemblyFilePath">The file path of the assembly to be loaded.</param>
        /// <returns>
        /// The resolved assembly reference.
        /// </returns>
        Assembly LoadAssemblyFromPath(string assemblyFilePath);
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
        public static bool IsRoot(this IAppRuntime appRuntime) => appRuntime?[IAppRuntime.IsRootKey] as bool? ?? false;

        /// <summary>
        /// Gets the identifier of the application.
        /// </summary>
        /// <param name="appRuntime">The app runtime to act on.</param>
        /// <returns>
        /// The identifier of the application.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? GetAppId(this IAppRuntime appRuntime) => appRuntime?[IAppRuntime.AppIdKey] as string;

        /// <summary>
        /// Gets the version of the application.
        /// </summary>
        /// <param name="appRuntime">The app runtime to act on.</param>
        /// <returns>
        /// The version of the application.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? GetAppVersion(this IAppRuntime appRuntime) => appRuntime?[IAppRuntime.AppVersionKey] as string;

        /// <summary>
        /// Gets the application identity.
        /// </summary>
        /// <param name="appRuntime">The app runtime to act on.</param>
        /// <returns>
        /// The application identity.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AppIdentity? GetAppIdentity(this IAppRuntime appRuntime) => appRuntime?[IAppRuntime.AppIdentityKey] as AppIdentity;

        /// <summary>
        /// Gets the running environment.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <returns>The running environment.</returns>
        public static string? GetEnvironment(this IAppRuntime appRuntime) => appRuntime?[IAppRuntime.EnvKey] as string;

        /// <summary>
        /// Gets a value indicating whether the running environment is development.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <returns>A value indicating whether the running environment is development.</returns>
        public static bool IsDevelopment(this IAppRuntime appRuntime) => string.Equals(EnvironmentName.Development, appRuntime.GetEnvironment(), StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the identifier of the application instance.
        /// </summary>
        /// <param name="appRuntime">The app runtime to act on.</param>
        /// <returns>
        /// The identifier of the application instance.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? GetAppInstanceId(this IAppRuntime appRuntime) => appRuntime?[IAppRuntime.AppInstanceIdKey] as string;

        /// <summary>
        /// Gets the full path of the file or folder. If the name is a relative path, it will be made relative to the application location.
        /// </summary>
        /// <param name="appRuntime">The app runtime to act on.</param>
        /// <param name="path">Relative or absolute path of the file or folder.</param>
        /// <returns>
        /// The full path of the file or folder.
        /// </returns>
        public static string GetFullPath(this IAppRuntime appRuntime, string? path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return appRuntime.GetAppLocation();
            }

            path = FileSystem.NormalizePath(path!);
            return Path.GetFullPath(Path.IsPathRooted(path) ? path : Path.Combine(appRuntime.GetAppLocation(), path));
        }
    }
}