// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataStreamReadService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataStreamReaderService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.DataStreams
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Shared application service contract for reading data streams.
    /// </summary>
    [SharedAppServiceContract]
    public interface IDataStreamReadService
    {
        /// <summary>
        /// Reads the data source and converts it to an enumeration of entities.
        /// </summary>
        /// <param name="dataStream">The <see cref="DataStream"/> containing the entities.</param>
        /// <param name="context">The data I/O context (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the deserialized entities.
        /// </returns>
        Task<object> ReadAsync(DataStream dataStream, IDataIOContext context = null, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Extension methods for <see cref="IDataStreamReadService"/>.
    /// </summary>
    public static class DataStreamReadServiceExtensions
    {
        /// <summary>
        /// Reads the data source and converts it to an enumeration of entities.
        /// </summary>
        /// <typeparam name="TRootObject">Type of the root object.</typeparam>
        /// <param name="dataStreamReadService">The <see cref="IDataStreamReadService"/> to act on.</param>
        /// <param name="dataStream">The <see cref="DataStream"/> containing the entities.</param>
        /// <param name="context">The data I/O context (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the deserialized entities.
        /// </returns>
        public static async Task<TRootObject> ReadAsync<TRootObject>(
            this IDataStreamReadService dataStreamReadService,
            DataStream dataStream,
            IDataIOContext context = null,
            CancellationToken cancellationToken = default)
        {
            context = (context ?? new DataIOContext()).WithRootObjectType(typeof(TRootObject));
            var result = (TRootObject)await dataStreamReadService.ReadAsync(dataStream, context, cancellationToken).PreserveThreadContext();
            return result;
        }
    }
}