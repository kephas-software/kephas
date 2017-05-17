// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataStreamWriter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data stream writer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.DataStreams
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Net.Mime;
    using Kephas.Serialization;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A data stream writer.
    /// </summary>
    [ProcessingPriority(Priority.Low)]
    public class DataStreamWriter : IDataStreamWriter
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
        /// Initializes a new instance of the <see cref="DataStreamWriter"/> class.
        /// </summary>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="mediaTypeProvider">The media type provider.</param>
        public DataStreamWriter(ISerializationService serializationService, IMediaTypeProvider mediaTypeProvider)
        {
            Requires.NotNull(serializationService, nameof(serializationService));
            Requires.NotNull(mediaTypeProvider, nameof(mediaTypeProvider));

            this.serializationService = serializationService;
            this.mediaTypeProvider = mediaTypeProvider;
        }

        /// <summary>
        /// Determines whether this instance can write to the specified data source.
        /// </summary>
        /// <param name="dataStream">The <see cref="DataStream"/>.</param>
        /// <returns><c>true</c> if this instance can write to the specified data source, otherwise <c>false</c>.</returns>
        public virtual bool CanWrite(DataStream dataStream)
        {
            return true;
        }

        /// <summary>
        /// Writes the entities to the data source.
        /// </summary>
        /// <param name="data">The entity or entities to be written.</param>
        /// <param name="dataStream">The <see cref="DataStream"/> where the entities should be written.</param>
        /// <param name="context">The data I/O context (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A task to await.
        /// </returns>
        public virtual async Task WriteAsync(object data, DataStream dataStream, IDataIOContext context = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Requires.NotNull(data, nameof(data));
            Requires.NotNull(dataStream, nameof(dataStream));

            cancellationToken.ThrowIfCancellationRequested();

            var serializationContext = new SerializationContext(this.serializationService, this.GetMediaType(dataStream))
                                           {
                                               RootObjectType = context?.RootObjectType ?? data.GetType()
                                           };
            var serializer = this.serializationService.GetSerializer(serializationContext);

            using (var writer = this.CreateEncodedStreamWriter(dataStream))
            {
                var rawResult = await serializer.SerializeAsync(data, serializationContext, cancellationToken).PreserveThreadContext();

                cancellationToken.ThrowIfCancellationRequested();

                await writer.WriteAsync(rawResult).PreserveThreadContext();

                cancellationToken.ThrowIfCancellationRequested();
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
        /// Creates the encoded stream writer.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <returns>
        /// The encoded stream writer.
        /// </returns>
        protected virtual StreamWriter CreateEncodedStreamWriter(DataStream dataSource)
        {
            var encoding = dataSource.Encoding;
            var writer = new StreamWriter(dataSource, encoding ?? Encoding.UTF8);
            return writer;
        }
    }
}