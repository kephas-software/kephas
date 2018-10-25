// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataKindAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data kind attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Setup.AttributedModel
{
    using System;

    using Kephas.Composition.Metadata;

    /// <summary>
    /// Attribute for indicating the data kind.
    /// </summary>
    public class DataKindAttribute : Attribute, IMetadataValue<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataKindAttribute"/> class.
        /// </summary>
        /// <param name="kind">The data kind.</param>
        public DataKindAttribute(string kind)
        {
            this.Value = kind;
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