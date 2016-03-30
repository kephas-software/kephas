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
        /// Initializes a new instance of the <see cref="AppManifestBase"/> class.
        /// </summary>
        protected AppManifestBase()
        {
            this.Initialize(this.GetDynamicTypeInfo().TypeInfo.Assembly);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppManifestBase"/> class.
        /// </summary>
        /// <param name="appAssembly">The application assembly containing the <see cref="AppManifestAttribute"/>.</param>
        protected AppManifestBase(Assembly appAssembly)
        {
            Contract.Requires(appAssembly != null);

            this.Initialize(appAssembly);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppManifestBase"/> class.
        /// </summary>
        /// <param name="appId">The identifier of the application.</param>
        /// <param name="appVersion">The application version.</param>
        protected AppManifestBase(string appId, Version appVersion)
            : base(isThreadSafe: true)
        {
            Contract.Requires(!string.IsNullOrEmpty(appId));
            Contract.Requires(appVersion != null);

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
        /// Initializes the application manifest from the <see cref="AppManifestAttribute"/> set in the provided application assembly.
        /// </summary>
        protected virtual void Initialize(Assembly appAssembly)
        {
            Contract.Requires(appAssembly != null);

            var appManifestAttribute = appAssembly.GetCustomAttribute<AppManifestAttribute>();
            if (appManifestAttribute == null)
            {
                throw new InvalidOperationException(string.Format(Strings.AppManifestBase_MissingAppManifestAttribute_Exception, appAssembly.FullName));
            }

            Version appVersion;
            var version = appManifestAttribute.AppVersion;
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

            this.Initialize(appManifestAttribute.AppId, appVersion);
        }

        /// <summary>
        /// Initializes the application manifest with the provided application ID and version.
        /// </summary>
        /// <param name="appId">The identifier of the application.</param>
        /// <param name="appVersion">The application version.</param>
        protected virtual void Initialize(string appId, Version appVersion)
        {
            Contract.Requires(!string.IsNullOrEmpty(appId));
            Contract.Requires(appVersion != null);

            this.AppId = appId;
            this.AppVersion = appVersion;
        }
    }
}