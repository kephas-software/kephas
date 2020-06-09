// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataStreamWriterMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data stream writer metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.DataStreams.Composition
{
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Services.Composition;

    /// <summary>
    /// Metadata for <see cref="IDataStreamWriter"/> services.
    /// </summary>
    public class DataStreamWriterMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataStreamWriterMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public DataStreamWriterMetadata(IDictionary<string, object?> metadata)
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