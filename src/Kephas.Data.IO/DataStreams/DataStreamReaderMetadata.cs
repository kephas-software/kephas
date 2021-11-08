// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataStreamReaderMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data stream reader metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.DataStreams
{
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Services;

    /// <summary>
    /// Metadata for <see cref="IDataStreamReader"/> services.
    /// </summary>
    public class DataStreamReaderMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataStreamReaderMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public DataStreamReaderMetadata(IDictionary<string, object?>? metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.SupportedMediaTypes = (string[])metadata.TryGetValue(nameof(this.SupportedMediaTypes));
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