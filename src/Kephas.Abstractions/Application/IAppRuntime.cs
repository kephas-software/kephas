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
    using System.Reflection;

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
        /// Gets a value indicating whether the application is the root of an application hierarchy.
        /// </summary>
        /// <returns>
        /// A value indicating whether the application is the root of an application hierarchy.
        /// </returns>
        bool IsRoot { get; }

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
        /// <returns>
        /// An enumeration of application assemblies.
        /// </returns>
        IEnumerable<Assembly> GetAppAssemblies();

        /// <summary>
        /// Gets the identifier of the application.
        /// </summary>
        /// <returns>
        /// The identifier of the application.
        /// </returns>
        string? GetAppId() => this[AppIdKey] as string;

        /// <summary>
        /// Gets the version of the application.
        /// </summary>
        /// <returns>
        /// The version of the application.
        /// </returns>
        string? GetAppVersion() => this[IAppRuntime.AppVersionKey] as string;

        /// <summary>
        /// Gets the application identity.
        /// </summary>
        /// <returns>
        /// The application identity.
        /// </returns>
        AppIdentity? GetAppIdentity() => this[IAppRuntime.AppIdentityKey] as AppIdentity;

        /// <summary>
        /// Gets the running environment.
        /// </summary>
        /// <returns>The running environment.</returns>
        string? GetEnvironment() => this[IAppRuntime.EnvKey] as string;

        /// <summary>
        /// Gets a value indicating whether the running environment is development.
        /// </summary>
        /// <returns>A value indicating whether the running environment is development.</returns>
        bool IsDevelopmentEnvironment() => string.Equals(EnvironmentName.Development, this.GetEnvironment(), StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the identifier of the application instance.
        /// </summary>
        /// <returns>
        /// The identifier of the application instance.
        /// </returns>
        string? GetAppInstanceId() => this[IAppRuntime.AppInstanceIdKey] as string;

        /// <summary>
        /// Gets the full path of the file or folder. If the name is a relative path, it will be made relative to the application location.
        /// </summary>
        /// <param name="path">Relative or absolute path of the file or folder.</param>
        /// <returns>
        /// The full path of the file or folder.
        /// </returns>
        string GetFullPath(string? path) => FileSystem.GetFullPath(path, this.GetAppLocation());
    }
}