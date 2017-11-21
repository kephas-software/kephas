// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataImportBehavior.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    [SharedAppServiceContract(AllowMultiple = true)]
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
        /// <param name="importEntityInfo">Information describing the entity to import.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        Task BeforeConvertEntityAsync(
            IEntityInfo importEntityInfo,
            IDataImportContext context,
            CancellationToken cancellationToken);

        /// <summary>
        /// Callback invoked before persisting the converted entity asynchronously.
        /// </summary>
        /// <param name="importEntityInfo">Information describing the entity to import.</param>
        /// <param name="targetEntityInfo">Information describing the converted entity to persist.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        Task BeforePersistEntityAsync(
            IEntityInfo importEntityInfo,
            IEntityInfo targetEntityInfo,
            IDataImportContext context,
            CancellationToken cancellationToken);

        /// <summary>
        /// Callback invoked after the converted entity has been persisted asynchronously.
        /// </summary>
        /// <param name="importEntityInfo">Information describing the entity to import.</param>
        /// <param name="targetEntityInfo">Information describing the converted entity to persist.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        Task AfterPersistEntityAsync(
            IEntityInfo importEntityInfo,
            IEntityInfo targetEntityInfo,
            IDataImportContext context,
            CancellationToken cancellationToken);
    }
}