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
    using Kephas.Model.Elements.Construction;

    /// <summary>
    /// Classifier definition for a value type.
    /// </summary>
    public class ValueType : ClassifierBase<IValueType, IValueTypeInfo>, IValueType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueType" /> class.
        /// </summary>
        /// <param name="elementInfo">Information describing the element.</param>
        /// <param name="modelSpace">The model space.</param>
        public ValueType(IValueTypeInfo elementInfo, IModelSpace modelSpace)
            : base(elementInfo, modelSpace)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the value type represents a primitive value, 
        /// like an integer or a string.
        /// </summary>
        public bool IsPrimitive { get; }

        /// <summary>
        /// Gets a value indicating whether the value type representing a complex value,
        /// like a structure consisting of multiple properties.
        /// </summary>
        /// <remarks>
        /// If a value type is not a simple type, then it is a complex type.
        /// </remarks>
        public bool IsComplex { get; }
    }
}