// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataTargetAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data target attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Setup.AttributedModel
{
    using System;

    using Kephas.Composition.Metadata;

    /// <summary>
    /// Attribute for indicating the data target.
    /// </summary>
    public class DataTargetAttribute : Attribute, IMetadataValue<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataTargetAttribute"/> class.
        /// </summary>
        /// <param name="target">The data target.</param>
        public DataTargetAttribute(string target)
        {
            this.Value = target;
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