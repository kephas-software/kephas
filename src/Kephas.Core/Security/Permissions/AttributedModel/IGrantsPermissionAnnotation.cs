// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGrantsPermissionAnnotation.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Permissions.AttributedModel
{
    using System;

    /// <summary>
    /// Marker interface for indicating granted permissions.
    /// </summary>
    public interface IGrantsPermissionAnnotation
    {
        /// <summary>
        /// Gets the types of the granted permissions.
        /// </summary>
        /// <value>
        /// The types of the granted permissions.
        /// </value>
        Type[] PermissionTypes { get; }
    }
}