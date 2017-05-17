// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataStreamReadService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the default data stream reader service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.DataStreams
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Data.IO.DataStreams.Composition;
    using Kephas.Data.IO.Resources;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// A default data stream reader service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultDataStreamReadService : IDataStreamReadService
    {
        /// <summary>
        /// The reader factories.
        /// </summary>
        private readonly ICollection<IExportFactory<IDataStreamReader, DataStreamReaderMetadata>> readerFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDataStreamReadService"/> class.
        /// </summary>
        /// <param name="readerFactories">The reader factories.</param>
        public DefaultDataStreamReadService(ICollection<IExportFactory<IDataStreamReader, DataStreamReaderMetadata>> readerFactories)
        {
            Requires.NotNull(readerFactories, nameof(readerFactories));

            this.readerFactories = readerFactories.OrderBy(f => f.Metadata.ProcessingPriority).ToList();
        }

        /// <summary>
        /// Reads the data source and converts it to an enumeration of client entities.
        /// </summary>
        /// <param name="dataStream">The <see cref="DataStream"/> containing the entities.</param>
        /// <param name="context">The data I/O context (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the deserialized entities.
        /// </returns>
        public Task<object> ReadAsync(DataStream dataStream, IDataIOContext context = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Requires.NotNull(dataStream, nameof(dataStream));

            var reader = this.TryGetReader(dataStream);
            if (reader == null)
            {
                throw new DataIOException(string.Format(Strings.DefaultDataStreamReadService_ReaderNotFound_Exception, dataStream));
            }

            return reader.ReadAsync(dataStream, context, cancellationToken);
        }

        /// <summary>
        /// Tries to get an <see cref="IDataStreamReader"/> service.
        /// </summary>
        /// <param name="dataStream">The <see cref="DataStream"/> containing the entities.</param>
        /// <returns>
        /// An IDataStreamReader.
        /// </returns>
        protected virtual IDataStreamReader TryGetReader(DataStream dataStream)
        {
            // TODO optimize
            var factory = this.readerFactories.FirstOrDefault(f => f.Metadata.SupportedMediaTypes?.Contains(dataStream.MediaType) ?? false);
            if (factory == null)
            {
                factory = this.readerFactories.FirstOrDefault(f => f.Metadata.SupportedMediaTypes == null);
            }

            return factory?.CreateExportedValue();
        }
    }
}