// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeValueTypeInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Runtime based constructor information for value types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using System.Reflection;

    using Kephas.Model.Elements.Construction;

    /// <summary>
    /// Runtime based constructor information for value types.
    /// </summary>
    public class RuntimeValueTypeInfo : RuntimeClassifierInfo, IValueTypeInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeValueTypeInfo"/> class.
        /// </summary>
        /// <param name="runtimeElement">The runtime value type information.</param>
        public RuntimeValueTypeInfo(TypeInfo runtimeElement)
            : base(runtimeElement)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the value type represents a primitive value, like an integer or
        /// a string.
        /// </summary>
        /// <value>
        /// <c>true</c> if this value type is primitive, <c>false</c> otherwise.
        /// </value>
        public bool IsPrimitive { get; internal set; }

        /// <summary>
        /// Convert this object into a string representation.
        /// </summary>
        /// <returns>
        /// A string that represents this object.
        /// </returns>
        public override string ToString()
        {
            var modifier = this.IsPrimitive ? "Primitive" : "Complex";
            return $"{modifier} {this.Name} ({this.RuntimeElement})";
        }
    }
}