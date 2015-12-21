// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValueTypeInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Specialized classifier information for value types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements.Construction
{
    /// <summary>
    /// Specialized classifier information for value types.
    /// </summary>
    public interface IValueTypeInfo : IClassifierInfo
    {
        /// <summary>
        /// Gets a value indicating whether the value type represents a primitive value, 
        /// like an integer or a string.
        /// </summary>
        bool IsPrimitive { get; }
    }
}