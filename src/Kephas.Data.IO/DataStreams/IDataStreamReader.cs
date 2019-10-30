// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataStreamReader.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataStreamReader interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.DataStreams
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Net.Mime;
    using Kephas.Services;

    /// <summary>
    /// Reader for <see cref="DataStream"/>s.
    /// </summary>
    [SingletonAppServiceContract(
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
        /// <param name="context">Optional. The data I/O context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the deserialized entities.
        /// </returns>
        Task<object> ReadAsync(DataStream dataStream, IDataIOContext context = null, CancellationToken cancellationToken = default);
    }
}