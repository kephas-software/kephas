﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaTypeMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the format metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Net.Mime.Composition
{
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services.Composition;

    /// <summary>
    /// Metadata for <see cref="IMediaType"/> services.
    /// </summary>
    public class MediaTypeMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaTypeMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public MediaTypeMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.SupportedMediaTypes = (string[])metadata.TryGetValue(nameof(this.SupportedMediaTypes));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaTypeMetadata" /> class.
        /// </summary>
        /// <param name="supportedMediaTypes">The supported media types.</param>
        /// <param name="processingPriority">The processing priority.</param>
        /// <param name="overridePriority">  The override priority.</param>
        /// <param name="optionalService">   <c>true</c> if the service is optional, <c>false</c> if not.</param>
        public MediaTypeMetadata(string[] supportedMediaTypes, int processingPriority = 0, int overridePriority = 0, bool optionalService = false)
            : base(processingPriority, overridePriority, optionalService)
        {
            Requires.NotNull(supportedMediaTypes, nameof(supportedMediaTypes));

            this.SupportedMediaTypes = supportedMediaTypes;
        }

        /// <summary>
        /// Gets the supported data formats.
        /// </summary>
        /// <value>
        /// The supported data formats.
        /// </value>
        public string[] SupportedMediaTypes { get; }
    }
}