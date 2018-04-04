// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaturalKeyAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the natural key attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.AttributedModel
{
    using System;

    /// <summary>
    /// Defines a natural key for the annotated entity.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public class NaturalKeyAttribute : KeyAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NaturalKeyAttribute"/> class.
        /// </summary>
        /// <param name="keyProperties">The key properties.</param>
        public NaturalKeyAttribute(string[] keyProperties)
            : base(null, KeyKind.Natural, keyProperties)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NaturalKeyAttribute"/> class.
        /// </summary>
        /// <param name="name">The key name.</param>
        /// <param name="keyProperties">The key properties.</param>
        public NaturalKeyAttribute(string name, string[] keyProperties)
            : base(name, KeyKind.Natural, keyProperties)
        {
        }
    }
}