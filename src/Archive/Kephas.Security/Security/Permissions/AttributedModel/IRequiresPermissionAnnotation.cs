// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRequiresPermissionAnnotation.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Permissions.AttributedModel
{
    using System;

    /// <summary>
    /// Marker interface for indicating required permissions.
    /// </summary>
    public interface IRequiresPermissionAnnotation
    {
        /// <summary>
        /// Gets the types of the required permissions.
        /// </summary>
        /// <value>
        /// The types of the required permissions.
        /// </value>
        Type[] PermissionTypes { get; }
    }
}