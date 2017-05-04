// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
        /// Gets a query over the entity type for the given query operationContext, if any is provided.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="queryOperationContext">Context for the query.</param>
        /// <returns>
        /// A query over the entity type.
        /// </returns>
        IQueryable<T> Query<T>(IQueryOperationContext queryOperationContext = null)
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
        /// Gets the entity extended information.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// The entity extended information.
        /// </returns>
        IEntityInfo GetEntityInfo(object entity);

        /// <summary>
        /// Attaches the entity to the data context.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// The entity extended information.
        /// </returns>
        IEntityInfo AttachEntity(object entity);

        /// <summary>
        /// Detaches the entity from the data context.
        /// </summary>
        /// <param name="entityInfo">The entity information.</param>
        /// <returns>
        /// The entity extended information.
        /// </returns>
        IEntityInfo DetachEntity(IEntityInfo entityInfo);
    }
}