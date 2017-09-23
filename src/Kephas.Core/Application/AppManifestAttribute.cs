// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppManifestAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Attribute for providing information for the application manifest.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Attribute for providing information for the application manifest.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class AppManifestAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppManifestAttribute"/> class.
        /// </summary>
        /// <param name="appId">Identifier for the application.</param>
        /// <param name="appVersion">The application version.</param>
        public AppManifestAttribute(string appId, string appVersion = null)
        {
            Requires.NotNullOrEmpty(appId, nameof(appId));

            this.AppId = appId;
            this.AppVersion = appVersion;
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
        public string AppVersion { get; }
    }
}