﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GrantsPermissionAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the grant permission attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Permissions.AttributedModel
{
    using System;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Attribute indicating that the permission to access/execute/use the decorated element is granted.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public class GrantsPermissionAttribute : Attribute, IGrantsPermissionAnnotation
    {
        /// <summary>
        /// The permission types metadata key.
        /// </summary>
        public const string PermissionTypesMetadataKey = "GrantsPermissionTypes";

        /// <summary>
        /// Initializes a new instance of the <see cref="GrantsPermissionAttribute"/> class.
        /// </summary>
        /// <param name="permissions">A variable-length parameters list containing the required permissions.</param>
        public GrantsPermissionAttribute(params Type[] permissions)
        {
            Requires.NotNullOrEmpty(permissions, nameof(permissions));

            this.PermissionTypes = permissions;
        }

        /// <summary>
        /// Gets the types of the granted permissions.
        /// </summary>
        /// <value>
        /// The types of the granted permissions.
        /// </value>
        [MetadataValue(PermissionTypesMetadataKey)]
        public Type[] PermissionTypes { get; }
    }
}