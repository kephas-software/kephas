// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataStreamWriteService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    /// Singleton application service contract for writing data streams.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IDataStreamWriteService
    {
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
        Task WriteAsync(object data, DataStream dataStream, IDataIOContext context = null, CancellationToken cancellationToken = default);
    }
}