// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataStreamWriter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataStreamWriter interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.DataStreams
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Net.Mime;
    using Kephas.Services;

    /// <summary>
    /// Writer for <see cref="DataStream"/>s.
    /// </summary>
    [SharedAppServiceContract(
        AllowMultiple = true,
        MetadataAttributes = new[] { typeof(SupportedMediaTypesAttribute) })]
    public interface IDataStreamWriter
    {
        /// <summary>
        /// Determines whether this instance can write to the specified data source.
        /// </summary>
        /// <param name="dataStream">The <see cref="DataStream"/>.</param>
        /// <returns><c>true</c> if this instance can write to the specified data source, otherwise <c>false</c>.</returns>
        bool CanWrite(DataStream dataStream);

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