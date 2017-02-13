// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueTypeAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the value type attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.AttributedModel
{
    using System;

    /// <summary>
    /// Attribute used to mark value types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class ValueTypeAttribute : ClassifierKindAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTypeAttribute"/> class.
        /// </summary>
        public ValueTypeAttribute()
            : base(typeof(IValueType))
        {
        }
    }
}