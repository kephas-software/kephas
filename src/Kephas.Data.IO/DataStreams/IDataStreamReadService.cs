// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataStreamReadService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataStreamReaderService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.DataStreams
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Shared application service contract for reading data streams.
    /// </summary>
    [SharedAppServiceContract]
    public interface IDataStreamReadService
    {
        /// <summary>
        /// Reads the data source and converts it to an enumeration of client entities.
        /// </summary>
        /// <param name="dataStream">The <see cref="DataStream"/> containing the entities.</param>
        /// <param name="context">The data I/O context (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the deserialized entities.
        /// </returns>
        Task<IEnumerable<object>> ReadAsync(DataStream dataStream, IDataIOContext context = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}