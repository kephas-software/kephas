// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DemandPermissionAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the demand permission attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization
{
    using System;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Attribute indicating the required permission to access/execute/use the decorated element.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class RequiresPermissionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiresPermissionAttribute"/> class.
        /// </summary>
        /// <param name="permissions">A variable-length parameters list containing the required permissions.</param>
        public RequiresPermissionAttribute(params string[] permissions)
        {
            Requires.NotNullOrEmpty(permissions, nameof(permissions));

            this.Permissions = permissions;
        }

        /// <summary>
        /// Gets the required permissions.
        /// </summary>
        /// <value>
        /// The required permissions.
        /// </value>
        public string[] Permissions { get; }
    }
}