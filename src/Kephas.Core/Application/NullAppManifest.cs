// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullAppManifest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the null application manifest class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using Kephas.Services;

    /// <summary>
    /// An application manifest returning the "null" ID and version 0.0.0.0.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullAppManifest : AppManifestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullAppManifest"/> class.
        /// </summary>
        public NullAppManifest()
            : base("null", VersionZero)
        {
        }
    }
}