// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaturalKeyAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the natural key attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.AttributedModel
{
    /// <summary>
    /// Defines a natural key for the annotated entity.
    /// </summary>
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