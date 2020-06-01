// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissionTypeAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the permission type attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Security.Authorization.AttributedModel
{
    using System;
    using Kephas.Model.AttributedModel;
    using Kephas.Reflection;
    using Kephas.Security.Authorization;
    using Kephas.Security.Authorization.Reflection;

    /// <summary>
    /// Attribute used to mark permission types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class PermissionTypeAttribute : ClassifierKindAttribute, IScoped, IToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionTypeAttribute"/> class.
        /// </summary>
        /// <param name="classifierName">Optional. Name of the classifier.</param>
        /// <param name="scoping">Optional. The scoping.</param>
        public PermissionTypeAttribute(string classifierName = null, Scoping scoping = Scoping.Global)
            : base(typeof(IPermissionType), classifierName)
        {
            this.Scoping = scoping;
        }

        /// <summary>
        /// Gets the permission scoping.
        /// </summary>
        /// <value>
        /// The permission scoping.
        /// </value>
        public Scoping Scoping { get; }

        /// <summary>
        /// Gets the token name.
        /// </summary>
        string IToken.TokenName => this.ClassifierName ?? string.Empty;
    }
}