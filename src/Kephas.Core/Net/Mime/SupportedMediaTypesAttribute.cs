// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SupportedMediaTypesAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the supported media types attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Net.Mime
{
    using System;

    using Kephas.Composition.Metadata;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Indicates the supported data formats for data stream readers and writers.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SupportedMediaTypesAttribute : Attribute, IMetadataValue<string[]>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedMediaTypesAttribute" /> class.
        /// </summary>
        /// <param name="supportedMediaTypes">The supported media types.</param>
        public SupportedMediaTypesAttribute(string[] supportedMediaTypes)
        {
            Requires.NotNull(supportedMediaTypes, nameof(supportedMediaTypes));
            //Requires.That(supportedMediaTypes.Length > 0);

            this.Value = supportedMediaTypes;
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
        public string[] Value { get; }
    }
}