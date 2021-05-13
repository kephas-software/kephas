// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissionScopeAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization.AttributedModel
{
    using System;

    /// <summary>
    /// Attribute marking a permission scope. This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// A type can be a permission scope, meaning that permissions assigned at its level are valid for the whole graph starting from that type.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public sealed class PermissionScopeAttribute : Attribute, IPermissionScopeAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionScopeAttribute"/> class.
        /// </summary>
        /// <param name="scopeName">Name of the scope.</param>
        public PermissionScopeAttribute(string scopeName)
        {
            this.ScopeName = scopeName;
        }

        /// <summary>
        /// Gets the name of the scope.
        /// </summary>
        /// <value>
        /// The name of the scope.
        /// </value>
        public string ScopeName { get; }
    }
}