// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataStreamWriter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data stream writer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.DataStreams
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

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
        private readonly ISerializationService serializationService;
        private readonly IMediaTypeProvider mediaTypeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStreamWriter"/> class.
        /// </summary>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="mediaTypeProvider">The media type provider.</param>
        public DataStreamWriter(
            ISerializationService serializationService,
            IMediaTypeProvider mediaTypeProvider)
        {
            serializationService = serializationService ?? throw new ArgumentNullException(nameof(serializationService));
            mediaTypeProvider = mediaTypeProvider ?? throw new System.ArgumentNullException(nameof(mediaTypeProvider));

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
        /// <param name="context">Optional. The data I/O context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public virtual async Task WriteAsync(object data, DataStream dataStream, IDataIOContext? context = null, CancellationToken cancellationToken = default)
        {
            data = data ?? throw new System.ArgumentNullException(nameof(data));
            dataStream = dataStream ?? throw new System.ArgumentNullException(nameof(dataStream));

            cancellationToken.ThrowIfCancellationRequested();

            Action<ISerializationContext> serializationConfig = ctx =>
                {
                    ctx.MediaType = this.GetMediaType(dataStream);
                    ctx.RootObjectType = context?.RootObjectType ?? data.GetType();
                    context?.SerializationConfig?.Invoke(ctx);
                };

            using var writer = this.CreateEncodedStreamWriter(dataStream);
            var rawResult = await this.serializationService.SerializeAsync(data, serializationConfig, cancellationToken).PreserveThreadContext();

            cancellationToken.ThrowIfCancellationRequested();

            await writer.WriteAsync(rawResult).PreserveThreadContext();

            cancellationToken.ThrowIfCancellationRequested();
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