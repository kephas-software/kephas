// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValueType.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   A value type denotes instances that are stored as values.
//   Value instances are not identifiable and cannot be referenced
//   from other instances, instead they are copied.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    /// <summary>
    /// A value type denotes instances that are stored as values.
    /// Value instances are not identifiable and cannot be referenced
    /// from other instances, instead they are copied.
    /// </summary>
    /// <remarks>
    /// The Kephas value types do not match necessarily the CLR value types. 
    /// Some CLR reference types are considered value types in Kephas, like <see cref="string"/>
    /// or <see cref="byte"/> array.
    /// Primitive value types store usually a single value (but this could also depend on the storage type),
    /// whereas complex value types store multiple values.
    /// Complex value types may however contain properties holding references to reference types.
    /// </remarks>
    public interface IValueType : IClassifier
    {
        /// <summary>
        /// Gets a value indicating whether the value type represents a primitive value, 
        /// like an integer or a string.
        /// </summary>
        bool IsPrimitive { get; }

        /// <summary>
        /// Gets a value indicating whether the value type representing a complex value,
        /// like a structure consisting of multiple properties.
        /// </summary>
        /// <remarks>
        /// If a value type is not a simple type, then it is a complex type.
        /// </remarks>
        bool IsComplex { get; }
    }
}