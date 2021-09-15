// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMediaTypeProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default format provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Net.Mime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Net.Mime.Composition;
    using Kephas.Resources;
    using Kephas.Services;

    /// <summary>
    /// A default format provider.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultMediaTypeProvider : IMediaTypeProvider
    {
        /// <summary>
        /// Dictionary of formats.
        /// </summary>
        private readonly IDictionary<string, Type> mediaTypeDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMediaTypeProvider"/> class.
        /// </summary>
        /// <param name="formatFactories">The format factories.</param>
        public DefaultMediaTypeProvider(ICollection<IExportFactory<IMediaType, MediaTypeMetadata>> formatFactories)
        {
            this.mediaTypeDictionary = new Dictionary<string, Type>();
            foreach (var factory in formatFactories.OrderBy(f => f.Metadata.ProcessingPriority))
            {
                var metadata = factory.Metadata;
                foreach (var mediaType in metadata.SupportedMediaTypes)
                {
                    this.mediaTypeDictionary[mediaType] = metadata.ServiceInstanceType;
                }
            }
        }

        /// <summary>
        /// Gets the media type based on the media type name.
        /// </summary>
        /// <param name="mediaTypeName">The media type name.</param>
        /// <param name="throwIfNotFound">True to throw if a format is not found (optional).</param>
        /// <returns>
        /// The media type or <c>null</c>.
        /// </returns>
        public Type GetMediaType(string mediaTypeName, bool throwIfNotFound = true)
        {
            var mediaType = this.mediaTypeDictionary.TryGetValue(mediaTypeName);
            if (mediaType == null && throwIfNotFound)
            {
                throw new KeyNotFoundException(string.Format(Strings.DefaultMediaTypeProvider_NotFound_Exception, mediaTypeName, typeof(IMediaType)));
            }

            return mediaType;
        }
    }
}