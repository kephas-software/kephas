// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppManifest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for the application manifest.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;

    using Kephas.Dynamic;
    using Kephas.Services;

    /// <summary>
    /// Contract for the application manifest.
    /// </summary>
    [SharedAppServiceContract]
    public interface IAppManifest : IExpando
    {
        /// <summary>
        /// Gets the identifier of the application.
        /// </summary>
        /// <value>
        /// The identifier of the application.
        /// </value>
        string AppId { get; }

        /// <summary>
        /// Gets the application version.
        /// </summary>
        /// <value>
        /// The application version.
        /// </value>
        Version AppVersion { get; }

        /// <summary>
        /// Gets the features provided by the application.
        /// </summary>
        /// <value>
        /// The application features.
        /// </value>
        IEnumerable<IFeatureInfo> Features { get; }
    }
}