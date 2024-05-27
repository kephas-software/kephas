// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionComparison.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Versioning
{
    /// <summary>Version comparison modes.</summary>
    public enum VersionComparison
    {
        /// <summary>
        /// Semantic version 2.0.1-rc comparison.
        /// </summary>
        Default,

        /// <summary>Compares only the version numbers.</summary>
        Version,

        /// <summary>
        /// Include Version number and Release labels in the compare.
        /// </summary>
        VersionRelease,

        /// <summary>Include all metadata during the compare.</summary>
        VersionReleaseMetadata,
    }
}