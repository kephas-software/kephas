// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissionInfoAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization.AttributedModel
{
    using System;

    using Kephas.Reflection;
    using Kephas.Security.Authorization.Reflection;

    /// <summary>
    /// Attribute indicating that the permission to access/execute/use the decorated element is granted.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class PermissionInfoAttribute : Attribute, IScoped, INamed
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionInfoAttribute"/> class.
        /// </summary>
        /// <param name="name">The permission name.</param>
        /// <param name="scoping">The permission scoping.</param>
        public PermissionInfoAttribute(string? name = null, Scoping scoping = Scoping.Global)
        {
            this.Scoping = scoping;
            this.Name = name ?? string.Empty;
        }

        /// <summary>
        /// Gets the scoping.
        /// </summary>
        /// <value>
        /// The scoping.
        /// </value>
        public Scoping Scoping { get; }

        /// <summary>
        /// Gets the name of the element.
        /// </summary>
        /// <value>
        /// The name of the element.
        /// </value>
        public string Name { get; }
    }
}