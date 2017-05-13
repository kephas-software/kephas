// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultFormatProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the default format provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Composition;
    using Kephas.Resources;
    using Kephas.Serialization.Composition;
    using Kephas.Services;

    /// <summary>
    /// A default format provider.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultFormatProvider : IFormatProvider
    {
        /// <summary>
        /// Dictionary of formats.
        /// </summary>
        private readonly IDictionary<string, Type> formatDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultFormatProvider"/> class.
        /// </summary>
        /// <param name="formatFactories">The format factories.</param>
        public DefaultFormatProvider(ICollection<IExportFactory<IFormat, FormatMetadata>> formatFactories)
        {
            this.formatDictionary = new Dictionary<string, Type>();
            foreach (var factory in formatFactories.OrderBy(f => f.Metadata.ProcessingPriority))
            {
                var metadata = factory.Metadata;
                foreach (var mediaType in metadata.SupportedMediaTypes)
                {
                    this.formatDictionary[mediaType] = metadata.AppServiceImplementationType;
                }
            }
        }

        /// <summary>
        /// Gets the format type based on the MIME type.
        /// </summary>
        /// <param name="mediaType">The media type.</param>
        /// <param name="throwIfNotFound">True to throw if a format is not found (optional).</param>
        /// <returns>
        /// The format type.
        /// </returns>
        public Type GetFormatType(string mediaType, bool throwIfNotFound = true)
        {
            var formatType = this.formatDictionary.TryGetValue(mediaType);
            if (formatType == null && throwIfNotFound)
            {
                throw new KeyNotFoundException(string.Format(Strings.DefaultFormatProvider_FormatNotFound_Exception, mediaType));
            }

            return formatType;
        }
    }
}