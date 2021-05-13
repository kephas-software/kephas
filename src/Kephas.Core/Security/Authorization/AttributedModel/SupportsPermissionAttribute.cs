// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SupportsPermissionAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the supports permission attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization.AttributedModel
{
    using System;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Attribute indicating that the decorated element supports the enumerated permissions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public class SupportsPermissionAttribute : Attribute, ISupportsPermissionAnnotation
    {
        /// <summary>
        /// The permission types metadata key.
        /// </summary>
        public const string PermissionTypesMetadataKey = "SupportsPermissionTypes";

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportsPermissionAttribute"/> class.
        /// </summary>
        /// <param name="permissions">A variable-length parameters list containing the required permissions.</param>
        public SupportsPermissionAttribute(params Type[] permissions)
        {
            Requires.NotNullOrEmpty(permissions, nameof(permissions));

            this.PermissionTypes = permissions;
        }

        /// <summary>
        /// Gets the types of the supported permissions.
        /// </summary>
        /// <value>
        /// The types of the supported permissions.
        /// </value>
        [MetadataValue(PermissionTypesMetadataKey)]
        public Type[] PermissionTypes { get; }
    }
}