// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityTypeAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the entity attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.AttributedModel
{
    using System;

    using Kephas.Model.AttributedModel;

    /// <summary>
    /// Attribute used to mark entity types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class EntityTypeAttribute : ClassifierKindAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityTypeAttribute"/> class.
        /// </summary>
        /// <param name="classifierName">Optional. Name of the classifier.</param>
        public EntityTypeAttribute(string? classifierName = null)
            : base(typeof(IEntityType), classifierName)
        {
        }
    }
}