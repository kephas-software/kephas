// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppManifest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for the application manifest.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
        string AppInstanceId { get; }

        /// <summary>
        /// Gets the features provided by the application.
        /// </summary>
        /// <value>
        /// The application features.
        /// </value>
        IEnumerable<IFeatureInfo> Features { get; }
    }

    /// <summary>
    /// Extension methods for <see cref="IAppManifest"/>.
    /// </summary>
    public static class AppManifestExtensions
    {
        /// <summary>
        /// Indicates whether the application manifest contains the indicated feature.
        /// </summary>
        /// <remarks>
        /// The name comparison is case insensitive.
        /// </remarks>
        /// <param name="appManifest">The application manifest to act on.</param>
        /// <param name="featureName">Name of the feature.</param>
        /// <returns>
        /// True if the application manifest contains the indicated feature, false otherwise.
        /// </returns>
        public static bool ContainsFeature(this IAppManifest appManifest, string featureName)
        {
            if (appManifest == null || featureName == null)
            {
                return false;
            }

            return appManifest.Features.Any(f => string.Equals(f.Name, featureName, StringComparison.OrdinalIgnoreCase));
        }
    }
}