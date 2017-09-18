// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataStreamReader.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataStreamReader interface.
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
    /// Reader for <see cref="DataStream"/>s.
    /// </summary>
    [SharedAppServiceContract(
        AllowMultiple = true,
        MetadataAttributes = new[] { typeof(SupportedMediaTypesAttribute) })]
    public interface IDataStreamReader
    {
        /// <summary>
        /// Determines whether this instance can read the specified <see cref="DataStream"/>.
        /// </summary>
        /// <param name="dataStream">The <see cref="DataStream"/>.</param>
        /// <returns><c>true</c> if this instance can read the specified <see cref="DataStream"/>, otherwise <c>false</c>.</returns>
        bool CanRead(DataStream dataStream);

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
}