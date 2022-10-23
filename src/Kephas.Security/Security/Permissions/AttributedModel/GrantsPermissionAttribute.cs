// --------------------------------------------------------------------------------------------------------------------
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

    using Kephas.Services;

    /// <summary>
    /// Attribute indicating that the permission to access/execute/use the decorated element is granted.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public class GrantsPermissionAttribute : Attribute, IGrantsPermissionAnnotation, IMetadataValue<Type[]>
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
            this.PermissionTypes = permissions ?? throw new ArgumentNullException(nameof(permissions));
        }

        /// <summary>
        /// Gets the types of the granted permissions.
        /// </summary>
        /// <value>
        /// The types of the granted permissions.
        /// </value>
        public Type[] PermissionTypes { get; }

        /// <summary>
        /// Gets the metadata name. If the name is not provided, it is inferred from the attribute type name.
        /// </summary>
        string? IMetadataValue.Name => PermissionTypesMetadataKey;

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        Type[] IMetadataValue<Type[]>.Value => this.PermissionTypes;
    }
}