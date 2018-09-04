// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataStreamReader.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data stream reader base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.DataStreams
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Net.Mime;
    using Kephas.Serialization;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Default class for <see cref="DataStream"/> readers.
    /// </summary>
    [ProcessingPriority(Priority.Low)]
    public class DataStreamReader : IDataStreamReader
    {
        /// <summary>
        /// The serialization service.
        /// </summary>
        private readonly ISerializationService serializationService;

        /// <summary>
        /// The media type provider.
        /// </summary>
        private readonly IMediaTypeProvider mediaTypeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStreamReader"/> class.
        /// </summary>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="mediaTypeProvider">The media type provider.</param>
        public DataStreamReader(ISerializationService serializationService, IMediaTypeProvider mediaTypeProvider)
        {
            Requires.NotNull(serializationService, nameof(serializationService));
            Requires.NotNull(mediaTypeProvider, nameof(mediaTypeProvider));

            this.serializationService = serializationService;
            this.mediaTypeProvider = mediaTypeProvider;
        }

        /// <summary>
        /// Determines whether this instance can read the specified <see cref="DataStream"/>.
        /// </summary>
        /// <param name="dataStream">The <see cref="DataStream"/>.</param>
        /// <returns><c>true</c> if this instance can read the specified <see cref="DataStream"/>, otherwise <c>false</c>.</returns>
        public virtual bool CanRead(DataStream dataStream)
        {
            return true;
        }

        /// <summary>
        /// Reads the data source and converts it to an enumeration of client entities.
        /// </summary>
        /// <param name="dataStream">The <see cref="DataStream"/> containing the entities.</param>
        /// <param name="context">The data I/O context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the deserialized entities.
        /// </returns>
        public virtual async Task<object> ReadAsync(DataStream dataStream, IDataIOContext context = null, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(dataStream, nameof(dataStream));

            cancellationToken.ThrowIfCancellationRequested();

            var serializationContext = new SerializationContext(this.serializationService, this.GetMediaType(dataStream))
                                           {
                                               RootObjectType = context?.RootObjectType ?? typeof(List<object>)
                                           };
            context?.SerializationContextConfig?.Invoke(serializationContext);
            var serializer = this.serializationService.GetSerializer(serializationContext);

            using (var reader = this.CreateEncodedStreamReader(dataStream))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var result = await serializer.DeserializeAsync(reader, serializationContext, cancellationToken).PreserveThreadContext();

                return result;
            }
        }

        /// <summary>
        /// Gets the media type for the <see cref="DataStream"/>.
        /// </summary>
        /// <param name="dataStream">The <see cref="DataStream"/>.</param>
        /// <returns>
        /// The media type.
        /// </returns>
        protected virtual Type GetMediaType(DataStream dataStream)
        {
            return this.mediaTypeProvider.GetMediaType(dataStream.MediaType);
        }

        /// <summary>
        /// Creates the encoded stream reader.
        /// </summary>
        /// <param name="dataStream">The data source.</param>
        /// <returns>
        /// The encoded stream reader.
        /// </returns>
        protected virtual StreamReader CreateEncodedStreamReader(DataStream dataStream)
        {
            var encoding = dataStream.Encoding;
            var reader = encoding == null
                             ? new StreamReader(dataStream, detectEncodingFromByteOrderMarks: true)
                             : new StreamReader(dataStream, encoding);
            return reader;
        }
    }
}