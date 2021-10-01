// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReturnTypeAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the return type attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime.AttributedModel
{
    using System;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Attribute for indicating the return type of an operation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
    public class ReturnTypeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnTypeAttribute"/> class.
        /// </summary>
        /// <param name="value">The type of the return value.</param>
        public ReturnTypeAttribute(Type value)
        {
            Requires.NotNull(value, nameof(value));

            this.Value = value;
        }

        /// <summary>
        /// Gets the type of the return value.
        /// </summary>
        /// <value>
        /// The type of the return value.
        /// </value>
        public Type Value { get; }
    }
}