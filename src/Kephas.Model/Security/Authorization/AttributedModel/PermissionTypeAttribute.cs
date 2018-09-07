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

    /// <summary>
    /// Attribute used to mark permission types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class PermissionTypeAttribute : ClassifierKindAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionTypeAttribute"/> class.
        /// </summary>
        /// <param name="classifierName">Optional. Name of the classifier.</param>
        public PermissionTypeAttribute(string classifierName = null)
            : base(typeof(IPermissionType), classifierName)
        {
        }
    }
}