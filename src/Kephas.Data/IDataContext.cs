// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract interface for data contexts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;
    using System.Linq;

    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Data.Store;
    using Kephas.Services;

    /// <summary>
    /// Contract interface for data contexts.
    /// </summary>
    [AppServiceContract(AllowMultiple = true, MetadataAttributes = new[] { typeof(SupportedDataStoreKindsAttribute) })]
    public interface IDataContext : IContext, IIdentifiable, IDisposable, IInitializable
    {
        /// <summary>
        /// Gets a query over the entity type for the given query operation context, if any is provided.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="queryConfig">Optional. The query configuration.</param>
        /// <returns>
        /// A query over the entity type.
        /// </returns>
        IQueryable<T> Query<T>(Action<IQueryOperationContext> queryConfig = null)
            where T : class;

        /// <summary>
        /// Creates the command with the provided type.
        /// </summary>
        /// <param name="commandType">The type of the command to be created.</param>
        /// <returns>
        /// The new command.
        /// </returns>
        IDataCommand CreateCommand(Type commandType);

        /// <summary>
        /// Gets the entity entry.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// The entity entry.
        /// </returns>
        IEntityEntry GetEntityEntry(object entity);

        /// <summary>
        /// Attaches the entity to the data context.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// The entity extended information.
        /// </returns>
        IEntityEntry Attach(object entity);

        /// <summary>
        /// Detaches the entity from the data context.
        /// </summary>
        /// <param name="entityEntry">The entity entry.</param>
        /// <returns>
        /// The entity extended information.
        /// </returns>
        IEntityEntry Detach(IEntityEntry entityEntry);
    }
}