// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppDependency.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAppDependency interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Reflection
{
    /// <summary>
    /// Interface for application dependency.
    /// </summary>
    public interface IAppDependency
    {
        /// <summary>
        /// Gets the name of the referenced app.
        /// </summary>
        /// <value>
        /// The name of the referenced app.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the version range.
        /// </summary>
        /// <value>
        /// The version range.
        /// </value>
        string? VersionRange { get; }
    }
}
