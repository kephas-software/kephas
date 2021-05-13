// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPermission.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization.Permissions
{
    /// <summary>
    /// Marker interface for permissions.
    /// </summary>
    public interface IPermission
    {
        /// <summary>
        /// Gets the token name.
        /// </summary>
        string TokenName { get; }

        /// <summary>
        /// Gets the permission scope.
        /// </summary>
        string? Scope { get; }

        /// <summary>
        /// Gets the sections within the scope.
        /// </summary>
        string[]? Sections { get; }
    }
}