// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataStreamReader.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    using Kephas.Reflection;
    using Kephas.Serialization;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    using IFormatProvider = Kephas.Serialization.IFormatProvider;

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
        /// The format provider.
        /// </summary>
        private readonly IFormatProvider formatProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStreamReader"/> class.
        /// </summary>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="formatProvider">The format provider.</param>
        public DataStreamReader(ISerializationService serializationService, IFormatProvider formatProvider)
        {
            Requires.NotNull(serializationService, nameof(serializationService));
            Requires.NotNull(formatProvider, nameof(formatProvider));

            this.serializationService = serializationService;
            this.formatProvider = formatProvider;
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
        public virtual async Task<IEnumerable<object>> ReadAsync(DataStream dataStream, IDataIOContext context = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Requires.NotNull(dataStream, nameof(dataStream));

            context.CheckCancellationRequested();

            var serializationContext = new SerializationContext(this.serializationService, this.GetFormatType(dataStream))
                                           {
                                               RootObjectType = typeof(List<object>)
                                           };
            var serializer = this.serializationService.GetSerializer(serializationContext);

            using (var reader = this.CreateEncodedStreamReader(dataStream))
            {
                var rawResult = await serializer.DeserializeAsync(reader, serializationContext, cancellationToken).PreserveThreadContext();

                context.CheckCancellationRequested();

                var result = rawResult.GetType().IsCollection()
                                 ? (IEnumerable<object>)rawResult
                                 : new[] { rawResult };
                return result;
            }
        }

        /// <summary>
        /// Gets the format type for the <see cref="DataStream"/>'s media type.
        /// </summary>
        /// <param name="dataStream">The <see cref="DataStream"/>.</param>
        /// <returns>
        /// The format type.
        /// </returns>
        protected virtual Type GetFormatType(DataStream dataStream)
        {
            return this.formatProvider.GetFormatType(dataStream.MediaType);
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