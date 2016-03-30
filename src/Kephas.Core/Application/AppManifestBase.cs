// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppManifestBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base class for the application manifest.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    using Kephas.Dynamic;
    using Kephas.Resources;

    /// <summary>
    /// Base class for the application manifest.
    /// </summary>
    public abstract class AppManifestBase : Expando, IAppManifest
    {
        /// <summary>
        /// The version zero.
        /// </summary>
        protected static readonly Version VersionZero = new Version("0.0.0.0");

        /// <summary>
        /// The application assembly.
        /// </summary>
        private Assembly appAssembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppManifestBase"/> class.
        /// </summary>
        protected AppManifestBase()
        {
            this.Initialize(this.AppAssembly);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppManifestBase"/> class.
        /// </summary>
        /// <param name="appAssembly">The application assembly containing the <see cref="AppManifestAttribute"/>.</param>
        protected AppManifestBase(Assembly appAssembly)
        {
            Contract.Requires(appAssembly != null);

            this.appAssembly = appAssembly;
            this.Initialize(appAssembly);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppManifestBase"/> class.
        /// </summary>
        /// <param name="appId">The identifier of the application.</param>
        /// <param name="appVersion">The application version.</param>
        protected AppManifestBase(string appId, Version appVersion = null)
            : base(isThreadSafe: true)
        {
            Contract.Requires(!string.IsNullOrEmpty(appId));

            this.Initialize(appId, appVersion);
        }

        /// <summary>
        /// Gets the identifier of the application.
        /// </summary>
        /// <value>
        /// The identifier of the application.
        /// </value>
        public string AppId { get; private set; }

        /// <summary>
        /// Gets the application version.
        /// </summary>
        /// <value>
        /// The application version.
        /// </value>
        public Version AppVersion { get; private set; }

        /// <summary>
        /// Gets the application assembly.
        /// </summary>
        public Assembly AppAssembly => this.appAssembly ?? (this.appAssembly = this.GetDynamicTypeInfo().TypeInfo.Assembly);

        /// <summary>
        /// Initializes the application manifest from the <see cref="AppManifestAttribute"/> set in the provided application assembly.
        /// </summary>
        protected virtual void Initialize(Assembly appAssembly)
        {
            Contract.Requires(appAssembly != null);

            this.Initialize(this.GetAppId(appAssembly), this.GetAppVersion(appAssembly), appAssembly);
        }

        /// <summary>
        /// Initializes the application manifest with the provided application ID and version.
        /// </summary>
        /// <param name="appId">The identifier of the application.</param>
        /// <param name="appVersion">The application version.</param>
        /// <param name="appAssembly">The application assembly containing the
        ///                           <see cref="AppManifestAttribute"/>.</param>
        protected virtual void Initialize(string appId, Version appVersion = null, Assembly appAssembly = null)
        {
            Contract.Requires(!string.IsNullOrEmpty(appId));

            this.AppId = appId;
            this.AppVersion = appVersion ?? this.GetAppVersion(appAssembly ?? this.AppAssembly);
        }

        /// <summary>
        /// Gets the application identifier.
        /// </summary>
        /// <param name="appAssembly">The application assembly containing the
        ///                           <see cref="AppManifestAttribute"/>.</param>
        /// <returns>
        /// The application identifier.
        /// </returns>
        private string GetAppId(Assembly appAssembly)
        {
            var appManifestAttribute = appAssembly.GetCustomAttribute<AppManifestAttribute>();
            if (appManifestAttribute == null)
            {
                throw new InvalidOperationException(
                    string.Format(Strings.AppManifestBase_MissingAppManifestAttribute_Exception, appAssembly.FullName));
            }

            return appManifestAttribute.AppId;
        }

        /// <summary>
        /// Gets the application version from the assembly.
        /// </summary>
        /// <param name="appAssembly">The application assembly containing the
        ///                           <see cref="AppManifestAttribute"/>.</param>
        /// <returns>
        /// The application version.
        /// </returns>
        private Version GetAppVersion(Assembly appAssembly)
        {
            var appManifestAttribute = appAssembly.GetCustomAttribute<AppManifestAttribute>();
            var version = appManifestAttribute?.AppVersion;

            Version appVersion;
            if (!Version.TryParse(version, out appVersion))
            {
                version = appAssembly.GetCustomAttribute<AssemblyVersionAttribute>()?.Version
                          ?? appAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version
                          ?? appAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

                if (!Version.TryParse(version, out appVersion))
                {
                    appVersion = VersionZero;
                }
            }

            return appVersion;
        }
    }
}