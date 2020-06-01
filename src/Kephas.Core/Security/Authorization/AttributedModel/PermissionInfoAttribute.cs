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
    public class PermissionInfoAttribute : Attribute, IScoped, IToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionInfoAttribute"/> class.
        /// </summary>
        /// <param name="tokenName">The permission's token name.</param>
        /// <param name="scoping">The permission scoping.</param>
        public PermissionInfoAttribute(string? tokenName = null, Scoping scoping = Scoping.Global)
        {
            this.Scoping = scoping;
            this.TokenName = tokenName ?? string.Empty;
        }

        /// <summary>
        /// Gets the scoping.
        /// </summary>
        /// <value>
        /// The scoping.
        /// </value>
        public Scoping Scoping { get; }

        /// <summary>
        /// Gets the name of the permission token.
        /// </summary>
        /// <value>
        /// The name of the permission token.
        /// </value>
        public string TokenName { get; }
    }
}