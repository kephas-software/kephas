// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GrantPermissionAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the grant permission attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization
{
    using System;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Attribute indicating that the permission to access/execute/use the decorated element is granted.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public class GrantsPermissionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrantsPermissionAttribute"/> class.
        /// </summary>
        /// <param name="permissions">A variable-length parameters list containing the required permissions.</param>
        public GrantsPermissionAttribute(params string[] permissions)
        {
            Requires.NotNullOrEmpty(permissions, nameof(permissions));

            this.Permissions = permissions;
        }

        /// <summary>
        /// Gets the granted permissions.
        /// </summary>
        /// <value>
        /// The granted permissions.
        /// </value>
        public string[] Permissions { get; }
    }
}