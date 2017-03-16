// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperatorAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the operator attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion
{
    using System;

    using Kephas.Composition.Metadata;
    using Kephas.Diagnostics.Contracts;

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
            Requires.NotNullOrEmpty(@operator, nameof(@operator));

            this.Value = @operator;
        }

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        object IMetadataValue.Value => this.Value;

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        public string Value { get; }
    }
}