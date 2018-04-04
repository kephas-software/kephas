// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataIOResourceService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default data i/o resource service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO
{
    using System.IO;
    using System.Security.Principal;

    using Kephas.Application;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// A default data I/O resource service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultDataIOResourceService : IDataIOResourceService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDataIOResourceService"/> class.
        /// </summary>
        /// <param name="appManifest">The application manifest.</param>
        public DefaultDataIOResourceService(IAppManifest appManifest)
        {
            Requires.NotNull(appManifest, nameof(appManifest));

            this.AppManifest = appManifest;
        }

        /// <summary>
        /// Gets the application manifest.
        /// </summary>
        /// <value>
        /// The application manifest.
        /// </value>
        public IAppManifest AppManifest { get; }

        /// <summary>
        /// Gets the data export path for the provided identity.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns>
        /// The data export path.
        /// </returns>
        public string GetResourcePath(IIdentity identity)
        {
            // TODO use a config file setting instead of the temp folder.
            var path = Path.Combine(Path.GetTempPath(), this.AppManifest.AppId);
            var userName = identity?.Name;
            if (string.IsNullOrEmpty(userName))
            {
                userName = "anonymous";
            }

            path = Path.Combine(path, userName);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }
    }
}