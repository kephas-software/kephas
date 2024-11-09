// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataImportBehaviorBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data import behavior base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Import
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Capabilities;
    using Kephas.Data.IO.DataStreams;
    using Kephas.Logging;

    /// <summary>
    /// Base implementation of a data import behavior.
    /// </summary>
    public abstract class DataImportBehaviorBase : Loggable, IDataImportBehavior
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
        public virtual Task BeforeReadDataSourceAsync(DataStream dataStream, IDataImportContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

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
        public Task AfterReadDataSourceAsync(
            DataStream dataStream,
            IDataImportContext context,
            IList<object> sourceEntities,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Callback invoked before converting the entity to import asynchronously.
        /// </summary>
        /// <param name="importEntityEntry">Information describing the entity to import.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public Task BeforeConvertEntityAsync(
            IEntityEntry importEntityEntry,
            IDataImportContext context,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

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
        public Task BeforePersistEntityAsync(
            IEntityEntry importEntityEntry,
            IEntityEntry targetEntityEntry,
            IDataImportContext context,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

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
        public Task AfterPersistEntityAsync(
            IEntityEntry importEntityEntry,
            IEntityEntry targetEntityEntry,
            IDataImportContext context,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}