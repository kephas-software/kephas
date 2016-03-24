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

    /// <summary>
    /// Base class for the application manifest.
    /// </summary>
    public abstract class AppManifestBase : Expando, IAppManifest
    {
        /// <summary>
        /// The application assembly.
        /// </summary>
        private Assembly appAssembly;

        /// <summary>
        /// The application version.
        /// </summary>
        private Version appVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppManifestBase"/> class.
        /// </summary>
        /// <param name="appId">The identifier of the application.</param>
        protected AppManifestBase(string appId)
            : base(isThreadSafe: true)
        {
            Contract.Requires(!string.IsNullOrEmpty(appId));

            this.AppId = appId;
        }

        /// <summary>
        /// Gets the identifier of the application.
        /// </summary>
        /// <value>
        /// The identifier of the application.
        /// </value>
        public string AppId { get; }

        /// <summary>
        /// Gets the application version.
        /// </summary>
        /// <value>
        /// The application version.
        /// </value>
        public virtual Version AppVersion => this.appVersion ?? (this.appVersion = new Version(this.AppAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion));

        /// <summary>
        /// Gets the application assembly from which the assembly attributes are queried.
        /// </summary>
        protected virtual Assembly AppAssembly => this.appAssembly ?? (this.appAssembly = this.GetDynamicTypeInfo().TypeInfo.Assembly);
    }
}