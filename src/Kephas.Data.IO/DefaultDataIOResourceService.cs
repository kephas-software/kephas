// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataIOResourceService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
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

    using Kephas.Services;

    /// <summary>
    /// A default data I/O resource service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultDataIOResourceService : IDataIOResourceService
    {
        /// <summary>
        /// Gets the data export path for the provided identity.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns>
        /// The data export path.
        /// </returns>
        public virtual string GetResourcePath(IIdentity identity)
        {
            // TODO use a config file setting instead of the temp folder.
            var path = Path.GetTempPath();
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