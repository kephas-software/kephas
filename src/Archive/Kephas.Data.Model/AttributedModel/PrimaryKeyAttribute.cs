// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PrimaryKeyAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the primary key attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.AttributedModel
{
    using System;

    /// <summary>
    /// Defines a primary key for the annotated entity.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class PrimaryKeyAttribute : KeyAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrimaryKeyAttribute"/> class.
        /// </summary>
        /// <param name="keyProperties">The key properties.</param>
        public PrimaryKeyAttribute(string[] keyProperties)
            : base(null, KeyKind.Primary, keyProperties)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimaryKeyAttribute"/> class.
        /// </summary>
        /// <param name="name">The key name.</param>
        /// <param name="keyProperties">The key properties.</param>
        public PrimaryKeyAttribute(string name, string[] keyProperties)
            : base(name, KeyKind.Primary, keyProperties)
        {
        }
    }
}