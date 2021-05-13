// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISupportsPermissionAnnotation.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization.AttributedModel
{
    using System;

    /// <summary>
    /// Marker interface for indicating supported permissions.
    /// </summary>
    public interface ISupportsPermissionAnnotation
    {
        /// <summary>
        /// Gets the types of the supported permissions.
        /// </summary>
        /// <value>
        /// The types of the supported permissions.
        /// </value>
        Type[] PermissionTypes { get; }
    }
}