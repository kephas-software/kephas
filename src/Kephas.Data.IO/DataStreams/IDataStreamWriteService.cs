﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataStreamWriteService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataStreamWriteService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.DataStreams
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Shared application service contract for writing data streams.
    /// </summary>
    [SharedAppServiceContract]
    public interface IDataStreamWriteService
    {
        /// <summary>
        /// Writes the entities to the data source.
        /// </summary>
        /// <param name="data">The entities to be written.</param>
        /// <param name="dataStream">The <see cref="DataStream"/> where the entities should be written.</param>
        /// <param name="context">The data I/O context (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A task to await.
        /// </returns>
        Task WriteAsync(IEnumerable<object> data, DataStream dataStream, IDataIOContext context = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}