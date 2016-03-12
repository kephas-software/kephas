// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueType.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Classifier definition for a value type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements
{
    using Kephas.Model.Construction;

    /// <summary>
    /// Classifier definition for a value type.
    /// </summary>
    public class ValueType : ClassifierBase<IValueType>, IValueType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueType" /> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The name.</param>
        public ValueType(IModelConstructionContext constructionContext, string name)
            : base(constructionContext, name)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the value type represents a primitive value, 
        /// like an integer or a string.
        /// </summary>
        public bool IsPrimitive { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the value type representing a complex value,
        /// like a structure consisting of multiple properties.
        /// </summary>
        /// <remarks>
        /// If a value type is not a simple type, then it is a complex type.
        /// </remarks>
        public bool IsComplex => !this.IsPrimitive;
    }
}