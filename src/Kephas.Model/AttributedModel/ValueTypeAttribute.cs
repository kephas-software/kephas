// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueTypeAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the value type attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.AttributedModel
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
        /// <param name="classifierName">Optional. Name of the classifier.</param>
        public ValueTypeAttribute(string classifierName = null)
            : base(typeof(IValueType), classifierName)
        {
        }
    }
}