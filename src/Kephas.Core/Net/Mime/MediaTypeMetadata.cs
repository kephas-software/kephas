// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaTypeMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the format metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Net.Mime
{
    using System.Collections.Generic;
    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Metadata for <see cref="IMediaType"/> services.
    /// </summary>
    public class MediaTypeMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaTypeMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public MediaTypeMetadata(IDictionary<string, object?>? metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.SupportedMediaTypes = (string[])metadata.TryGetValue(nameof(this.SupportedMediaTypes));
            this.SupportedFileExtensions = (string[])metadata.TryGetValue(nameof(this.SupportedFileExtensions));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaTypeMetadata" /> class.
        /// </summary>
        /// <param name="supportedMediaTypes">The supported media types.</param>
        /// <param name="supportedFileExtensions">Optional. The supported file extensions.</param>
        /// <param name="processingPriority">Optional. The processing priority.</param>
        /// <param name="overridePriority">Optional. The override priority.</param>
        public MediaTypeMetadata(string[] supportedMediaTypes, string[]? supportedFileExtensions = null, Priority processingPriority = 0, Priority overridePriority = 0)
            : base(processingPriority, overridePriority)
        {
            Requires.NotNull(supportedMediaTypes, nameof(supportedMediaTypes));

            this.SupportedMediaTypes = supportedMediaTypes;
            this.SupportedFileExtensions = supportedFileExtensions;
        }

        /// <summary>
        /// Gets the supported data formats.
        /// </summary>
        /// <value>
        /// The supported data formats.
        /// </value>
        public string[] SupportedMediaTypes { get; }

        /// <summary>
        /// Gets the supported file extensions.
        /// </summary>
        /// <value>
        /// The supported file extensions.
        /// </value>
        public string[]? SupportedFileExtensions { get; }
    }
}