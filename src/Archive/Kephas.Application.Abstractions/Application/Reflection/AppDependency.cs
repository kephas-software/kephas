// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppDependency.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application dependency class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Reflection
{
    /// <summary>
    /// An application dependency.
    /// </summary>
    public class AppDependency : IAppDependency
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDependency"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="versionRange">The version range.</param>
        public AppDependency(string name, string? versionRange = null)
        {
            this.Name = name;
            this.VersionRange = versionRange;
        }

        /// <summary>
        /// Gets the name of the referenced app.
        /// </summary>
        /// <value>
        /// The name of the referenced app.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the version range.
        /// </summary>
        /// <value>
        /// The version range.
        /// </value>
        public string? VersionRange { get; }
    }
}
