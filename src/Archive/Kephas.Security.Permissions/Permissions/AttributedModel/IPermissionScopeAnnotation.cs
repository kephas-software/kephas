// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPermissionScopeAnnotation.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Permissions.AttributedModel
{
    /// <summary>
    /// Marker interface for permission scope annotations.
    /// </summary>
    public interface IPermissionScopeAnnotation
    {
        /// <summary>
        /// Gets the name of the permission scope.
        /// </summary>
        /// <value>
        /// The name of the permission scope.
        /// </value>
        string ScopeName { get; }
    }
}