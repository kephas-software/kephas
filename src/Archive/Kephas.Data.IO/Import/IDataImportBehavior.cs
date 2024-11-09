// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataImportBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataImportBehavior interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Import
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Capabilities;
    using Kephas.Data.IO.DataStreams;
    using Kephas.Services;

    /// <summary>
    /// Application service contract for adding behaviors to the data import.
    /// </summary>
    [SingletonAppServiceContract(AllowMultiple = true)]
    public interface IDataImportBehavior
    {
        /// <summary>
        /// Callback invoked before reading the data source asynchronously.
        /// </summary>
        /// <param name="dataStream">The data stream.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        Task BeforeReadDataSourceAsync(
            DataStream dataStream,
            IDataImportContext context,
            CancellationToken cancellationToken);

        /// <summary>
        /// Callback invoked before reading the data source asynchronously.
        /// </summary>
        /// <param name="dataStream">The data stream.</param>
        /// <param name="context">The context.</param>
        /// <param name="sourceEntities">The source entities as deserialized from the data stream.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        Task AfterReadDataSourceAsync(
            DataStream dataStream,
            IDataImportContext context,
            IList<object> sourceEntities,
            CancellationToken cancellationToken);

        /// <summary>
        /// Callback invoked before converting the entity to import asynchronously.
        /// </summary>
        /// <param name="importEntityEntry">Information describing the entity to import.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        Task BeforeConvertEntityAsync(
            IEntityEntry importEntityEntry,
            IDataImportContext context,
            CancellationToken cancellationToken);

        /// <summary>
        /// Callback invoked before persisting the converted entity asynchronously.
        /// </summary>
        /// <param name="importEntityEntry">Information describing the entity to import.</param>
        /// <param name="targetEntityEntry">Information describing the converted entity to persist.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        Task BeforePersistEntityAsync(
            IEntityEntry importEntityEntry,
            IEntityEntry targetEntityEntry,
            IDataImportContext context,
            CancellationToken cancellationToken);

        /// <summary>
        /// Callback invoked after the converted entity has been persisted asynchronously.
        /// </summary>
        /// <param name="importEntityEntry">Information describing the entity to import.</param>
        /// <param name="targetEntityEntry">Information describing the converted entity to persist.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        Task AfterPersistEntityAsync(
            IEntityEntry importEntityEntry,
            IEntityEntry targetEntityEntry,
            IDataImportContext context,
            CancellationToken cancellationToken);
    }
}