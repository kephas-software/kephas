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

    using Kephas.Services;

    /// <summary>
    /// Indicates the supported file extensions for the annotated media type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SupportedFileExtensionsAttribute : Attribute, IMetadataValue<string[]>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedFileExtensionsAttribute" /> class.
        /// </summary>
        /// <param name="supportedFileExtensions">The supported file extensions.</param>
        public SupportedFileExtensionsAttribute(string[] supportedFileExtensions)
        {
            this.Value = supportedFileExtensions ?? throw new ArgumentNullException(nameof(supportedFileExtensions));
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