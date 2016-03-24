// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppManifestBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the application base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics;
    using Kephas.Logging;
    using Kephas.Resources;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for the application manifest.
    /// </summary>
    public abstract class AppManifestBase : IAppManifest
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
        {
            Contract.Requires(!string.IsNullOrEmpty(appId));

            this.AppId = appId;
            this.appAssembly = this.GetDynamicTypeInfo().TypeInfo.Assembly;
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
        public virtual Version AppVersion => this.appVersion ?? (this.appVersion = new Version(this.appAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion));
    }
}