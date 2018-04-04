// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitialDataKindAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the initial data kind attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Initialization.AttributedModel
{
    using System;

    using Kephas.Composition.Metadata;

    /// <summary>
    /// Attribute for indicating the initial data kind.
    /// </summary>
    public class InitialDataKindAttribute : Attribute, IMetadataValue<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitialDataKindAttribute"/> class.
        /// </summary>
        /// <param name="kind">The data kind.</param>
        public InitialDataKindAttribute(string kind)
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