// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperatorAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the operator attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion
{
    using System;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection;

    /// <summary>
    /// Metadata attribute providing the operator.
    /// </summary>
    public class OperatorAttribute : Attribute, IMetadataValue<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperatorAttribute"/> class.
        /// </summary>
        /// <param name="operator">The operator.</param>
        public OperatorAttribute(string @operator)
        {
            if (string.IsNullOrEmpty(@operator)) throw new ArgumentException("Value must not be null or empty.", nameof(@operator));

            this.Value = @operator;
        }

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        public string Value { get; }
    }
}