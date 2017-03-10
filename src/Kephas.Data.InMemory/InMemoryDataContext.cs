// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryDataContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the in memory data context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.InMemory
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Kephas.Data.Commands.Factory;
    using Kephas.Data.Store;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Client data context managing.
    /// </summary>
    [SupportedDataStoreKinds(DataStoreKind.InMemory)]
    public class InMemoryDataContext : DataContextBase
    {
        /// <summary>
        /// The internal cache.
        /// </summary>
        private readonly List<object> cache = new List<object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryDataContext"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="dataCommandProvider">The data command provider.</param>
        public InMemoryDataContext(IAmbientServices ambientServices, IDataCommandProvider dataCommandProvider)
            : base(ambientServices, dataCommandProvider)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Contract.Requires(dataCommandProvider != null);
        }

        /// <summary>
        /// Gets a query over the entity type for the given query operationContext, if any is provided.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="queryOperationContext">Context for the query.</param>
        /// <returns>
        /// A query over the entity type.
        /// </returns>
        public override IQueryable<T> Query<T>(IQueryOperationContext queryOperationContext = null)
        {
            return this.cache.OfType<T>().AsQueryable();
        }

        /// <summary>
        /// Gets or add a cacheable item.
        /// </summary>
        /// <param name="operationContext">Context for the operation.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="isNew"><c>true</c> if the entity is new.</param>
        /// <returns>
        /// The or add cached item.
        /// </returns>
        internal object GetOrAddCacheableItem(IDataOperationContext operationContext, object entity, bool isNew)
        {
            Contract.Requires(entity != null);

            if (isNew)
            {
                this.cache.Add(entity);
                return entity;
            }

            var identifiable = this.TryGetCapability<IIdentifiable>(entity, operationContext);
            var entityId = identifiable?.Id;
            if (identifiable == null || Id.IsUnsetValue(entityId))
            {
                this.cache.Add(entity);
                return entity;
            }

            var entityType = entity.GetType();
            var existingEntity = this.cache.FirstOrDefault(e => e.GetType() == entityType && entityId.Equals(this.TryGetCapability<IIdentifiable>(e, operationContext)?.Id));
            if (existingEntity != null)
            {
                return existingEntity;
            }

            this.cache.Add(entity);
            return entity;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c>false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            this.cache.Clear();
        }
    }
}