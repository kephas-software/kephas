// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SupportedMediaTypesAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the supported media types attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Net.Mime
{
    using System;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection;

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
            Requires.NotNullOrEmpty(supportedMediaTypes, nameof(supportedMediaTypes));

            this.Value = supportedMediaTypes;
        }

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        public string[] Value { get; }
    }
}