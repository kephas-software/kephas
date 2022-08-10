// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataStreamWriteService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default data stream write service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.DataStreams
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.IO.Resources;
    using Kephas.Injection;
    using Kephas.Services;

    /// <summary>
    /// A default data stream reader service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultDataStreamWriteService : IDataStreamWriteService
    {
        /// <summary>
        /// The writer factories.
        /// </summary>
        private readonly IFactoryEnumerable<IDataStreamWriter, DataStreamWriterMetadata> writerFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDataStreamWriteService"/> class.
        /// </summary>
        /// <param name="writerFactories">The writer factories.</param>
        public DefaultDataStreamWriteService(ICollection<IExportFactory<IDataStreamWriter, DataStreamWriterMetadata>> writerFactories)
        {
            writerFactories = writerFactories ?? throw new System.ArgumentNullException(nameof(writerFactories));

            this.writerFactories = writerFactories.Order();
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
        public Task WriteAsync(object data, DataStream dataStream, IDataIOContext context = null, CancellationToken cancellationToken = default)
        {
            dataStream = dataStream ?? throw new System.ArgumentNullException(nameof(dataStream));

            var writer = this.TryGetWriter(dataStream);
            if (writer == null)
            {
                throw new DataIOException(string.Format(Strings.DefaultDataStreamWriteService_WriterNotFound_Exception, dataStream));
            }

            return writer.WriteAsync(data, dataStream, context, cancellationToken);
        }

        /// <summary>
        /// Tries to get an <see cref="IDataStreamWriter"/> service.
        /// </summary>
        /// <param name="dataStream">The <see cref="DataStream"/> containing the entities.</param>
        /// <returns>
        /// An IDataStreamWriter.
        /// </returns>
        protected virtual IDataStreamWriter TryGetWriter(DataStream dataStream)
        {
            // TODO optimize
            var factory = this.writerFactories.FirstOrDefault(f => f.Metadata.SupportedMediaTypes?.Contains(dataStream.MediaType) ?? false);
            if (factory == null)
            {
                factory = this.writerFactories.FirstOrDefault(f => f.Metadata.SupportedMediaTypes == null);
            }

            return factory?.CreateExportedValue();
        }
    }
}