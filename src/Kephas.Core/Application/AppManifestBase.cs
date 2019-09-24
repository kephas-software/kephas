// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppManifestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base class for the application manifest.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Kephas.Application.Reflection;
    using Kephas.Diagnostics.Contracts;
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
        protected internal static readonly Version VersionZero = new Version("0.0.0.0");

        /// <summary>
        /// The application assembly.
        /// </summary>
        private Assembly appAssembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppManifestBase"/> class.
        /// </summary>
        /// <param name="features">The application features.</param>
        protected AppManifestBase(IEnumerable<IFeatureInfo> features = null)
        {
            this.Initialize(this.AppAssembly, features);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppManifestBase"/> class.
        /// </summary>
        /// <param name="appAssembly">The application assembly containing the <see cref="AppManifestAttribute"/>.</param>
        /// <param name="features">The application features.</param>
        protected AppManifestBase(Assembly appAssembly, IEnumerable<IFeatureInfo> features = null)
        {
            Requires.NotNull(appAssembly, nameof(appAssembly));

            this.appAssembly = appAssembly;
            this.Initialize(appAssembly, features);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppManifestBase"/> class.
        /// </summary>
        /// <param name="appId">The identifier of the application.</param>
        /// <param name="appVersion">The application version.</param>
        /// <param name="features">The application features.</param>
        protected AppManifestBase(string appId, Version appVersion = null, IEnumerable<IFeatureInfo> features = null)
            : base(isThreadSafe: true)
        {
            Requires.NotNullOrEmpty(appId, nameof(appId));

            this.Initialize(appId, appVersion, features: features);
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
        /// Gets the identifier of the application instance.
        /// </summary>
        /// <remarks>
        /// This identifier is provided to uniquely identify an instance
        /// of the application across the distributed environment.
        /// This can be potentially used in message exchanging.
        /// </remarks>
        /// <value>
        /// The identifier of the application instance.
        /// </value>
        public virtual string AppInstanceId { get; private set; }

        /// <summary>
        /// Gets the features provided by the application.
        /// </summary>
        /// <value>
        /// The application features.
        /// </value>
        public IEnumerable<IFeatureInfo> Features { get; internal set; }

        /// <summary>
        /// Gets the application assembly.
        /// </summary>
        public Assembly AppAssembly => this.appAssembly ?? (this.appAssembly = this.GetRuntimeTypeInfo().TypeInfo.Assembly);

        /// <summary>
        /// Initializes the application manifest from the <see cref="AppManifestAttribute"/> set in the
        /// provided application assembly.
        /// </summary>
        /// <param name="appAssembly">The application assembly containing the <see cref="AppManifestAttribute"/>.</param>
        /// <param name="features">The application features.</param>
        protected virtual void Initialize(Assembly appAssembly, IEnumerable<IFeatureInfo> features)
        {
            Requires.NotNull(appAssembly, nameof(appAssembly));

            this.Initialize(this.GetAppId(appAssembly), this.GetAppVersion(appAssembly), appAssembly, features);
        }

        /// <summary>
        /// Initializes the application manifest with the provided application ID and version.
        /// </summary>
        /// <param name="appId">The identifier of the application.</param>
        /// <param name="appVersion">The application version.</param>
        /// <param name="appAssembly">
        /// The application assembly containing the <see cref="AppManifestAttribute"/>.
        /// </param>
        /// <param name="features">The application features.</param>
        protected virtual void Initialize(string appId, Version appVersion = null, Assembly appAssembly = null, IEnumerable<IFeatureInfo> features = null)
        {
            Requires.NotNullOrEmpty(appId, nameof(appId));

            this.AppId = appId;
            this.AppVersion = appVersion ?? this.GetAppVersion(appAssembly ?? this.AppAssembly);
            this.Features = features ?? new IFeatureInfo[0];
            this.AppInstanceId = $"{this.AppId}-{this.AppVersion}-{Guid.NewGuid()}";
        }

        /// <summary>
        /// Gets the application identifier.
        /// </summary>
        /// <param name="appAssembly">The application assembly containing the
        ///                           <see cref="AppManifestAttribute"/>.</param>
        /// <returns>
        /// The application identifier.
        /// </returns>
        protected virtual string GetAppId(Assembly appAssembly)
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
        protected virtual Version GetAppVersion(Assembly appAssembly)
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