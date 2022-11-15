// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequiresPermissionAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the requires permission attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Permissions.AttributedModel
{
    using System;

    using Kephas.Injection;

    /// <summary>
    /// Attribute indicating the required permission to access/execute/use the decorated element.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class RequiresPermissionAttribute : Attribute, IRequiresPermissionAnnotation, IMetadataValue<Type[]>
    {
        /// <summary>
        /// The permission types metadata key.
        /// </summary>
        public const string PermissionTypesMetadataKey = "RequiresPermissionTypes";

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiresPermissionAttribute"/> class.
        /// </summary>
        /// <param name="permissions">A variable-length parameters list containing the required permissions.</param>
        public RequiresPermissionAttribute(params Type[] permissions)
        {
            this.PermissionTypes = permissions ?? throw new ArgumentNullException(nameof(permissions));
        }

        /// <summary>
        /// Gets the types of the required permissions.
        /// </summary>
        /// <value>
        /// The types of the required permissions.
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