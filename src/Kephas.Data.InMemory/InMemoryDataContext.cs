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
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Data.Caching;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands.Factory;
    using Kephas.Data.InMemory.Caching;
    using Kephas.Data.Store;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;
    using Kephas.Serialization;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Client data context managing.
    /// </summary>
    /// <remarks>
    /// For the connection string, use something like in the example below:
    /// ConnectionString=UseSharedCache=true|false;InitialData=(JSON serialized data).
    /// </remarks>
    [SupportedDataStoreKinds(DataStoreKind.InMemory)]
    public class InMemoryDataContext : DataContextBase
    {
        /// <summary>
        /// The shared cache.
        /// </summary>
        private static readonly SharedDataContextCache SharedCache = new SharedDataContextCache();

        /// <summary>
        /// The working cache.
        /// </summary>
        private IDataContextCache workingCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryDataContext"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="dataCommandProvider">The data command provider.</param>
        /// <param name="serializationService">The serialization service.</param>
        public InMemoryDataContext(IAmbientServices ambientServices, IDataCommandProvider dataCommandProvider, ISerializationService serializationService)
            : base(ambientServices, dataCommandProvider)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(dataCommandProvider, nameof(dataCommandProvider));
            Requires.NotNull(serializationService, nameof(serializationService));

            this.SerializationService = serializationService;
        }

        /// <summary>
        /// Gets the serialization service.
        /// </summary>
        /// <value>
        /// The serialization service.
        /// </value>
        public ISerializationService SerializationService { get; }

        /// <summary>
        /// Gets the working cache.
        /// </summary>
        /// <value>
        /// The working cache.
        /// </value>
        protected internal IDataContextCache WorkingCache
        {
            get
            {
                this.InitializationMonitor.AssertIsCompletedSuccessfully();
                return this.workingCache;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to use the shared cache or not.
        /// </summary>
        /// <value>
        /// True if shared cache is used, false if not.
        /// </value>
        protected bool UseSharedCache { get; private set; }

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
            return this.WorkingCache.Values.Select(ei => ei.Entity).OfType<T>().AsQueryable();
        }

        /// <summary>
        /// Gets the entity extended information.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// The entity extended information.
        /// </returns>
        public override IEntityInfo GetEntityInfo(object entity)
        {
            // do not use here the WorkingCache property, because during initialization
            // the InitializationMonitor is not in the Completed state yet.
            var entityInfo = this.workingCache?.Values.FirstOrDefault(ei => ei.Entity == entity);
            return entityInfo;
        }

        /// <summary>
        /// Gets or add a cacheable item.
        /// </summary>
        /// <param name="operationContext">Context for the operation.</param>
        /// <param name="entityInfo">The entity information.</param>
        /// <returns>
        /// The or add cached item.
        /// </returns>
        internal IEntityInfo GetOrAddCacheableItem(IDataOperationContext operationContext, IEntityInfo entityInfo)
        {
            Requires.NotNull(entityInfo, nameof(entityInfo));

            var entity = entityInfo.Entity;
            var changeState = entityInfo.ChangeState;

            if (changeState == ChangeState.Added || changeState == ChangeState.AddedOrChanged)
            {
                this.Add(entityInfo);
                return entityInfo;
            }

            var identifiable = this.TryGetCapability<IIdentifiable>(entity, operationContext);
            var entityId = identifiable?.Id;
            if (identifiable == null || Id.IsUnsetValue(entityId))
            {
                this.Add(entityInfo);
                return entityInfo;
            }

            var entityType = entity.GetType();
            var existingEntry =
                this.WorkingCache.Values.FirstOrDefault(
                    e =>
                        e.Entity == entity
                        || (e.Entity.GetType() == entityType
                            && entityId.Equals(this.TryGetCapability<IIdentifiable>(e.Entity, operationContext)?.Id)));
            if (existingEntry != null)
            {
                return existingEntry;
            }

            this.Add(entityInfo);
            return entityInfo;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c>false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.UseSharedCache)
            {
                this.LocalCache.Clear();
            }
        }

        /// <summary>
        /// Initializes the <see cref="InMemoryDataContext"/>.
        /// </summary>
        /// <param name="dataInitializationContext">The data initialization context.</param>
        protected override void Initialize(IDataInitializationContext dataInitializationContext)
        {
            var config = dataInitializationContext?.Configuration;
            var connectionStringValues = string.IsNullOrWhiteSpace(config?.ConnectionString)
                                             ? new Dictionary<string, string>()
                                             : ConnectionStringParser.Parse(config.ConnectionString);

            bool.TryParse(connectionStringValues.TryGetValue("UseSharedCache"), out var useSharedCache);
            this.UseSharedCache = useSharedCache;
            this.workingCache = useSharedCache ? SharedCache : this.LocalCache;

            var serializedData = connectionStringValues.TryGetValue("InitialData");
            this.InitializeData(serializedData);

            this.InitializeData(config.GetInitialData());
            this.InitializeData(dataInitializationContext?.InitializationContext?.GetInitialData());
        }

        /// <summary>
        /// Adds an item to the internal cache.
        /// </summary>
        /// <param name="item">The item to add.</param>
        private void Add(IEntityInfo item)
        {
            if (this.UseSharedCache)
            {
                SharedCache.TryAdd(item.Id, item);
            }
            else
            {
                this.LocalCache.Add(item.Id, item);
            }
        }

        /// <summary>
        /// Initializes the data.
        /// </summary>
        /// <param name="initialData">The initial data.</param>
        private void InitializeData(IEnumerable<IEntityInfo> initialData)
        {
            if (initialData == null)
            {
                return;
            }

            var operationContext = new DataOperationContext(this);
            foreach (var entityInfo in initialData)
            {
                this.GetOrAddCacheableItem(operationContext, entityInfo);
            }
        }

        /// <summary>
        /// Initializes the data from the provided connection string.
        /// </summary>
        /// <param name="serializedData">The serialized data.</param>
        private void InitializeData(string serializedData)
        {
            if (string.IsNullOrWhiteSpace(serializedData))
            {
                return;
            }

            var data = this.SerializationService.JsonDeserializeAsync(serializedData).GetResultNonLocking(TimeSpan.FromMinutes(1));

            var operationContext = new DataOperationContext(this);
            if (data.GetType().IsCollection())
            {
                foreach (var entity in (IEnumerable)data)
                {
                    this.GetOrAddCacheableItem(operationContext, this.CreateEntityInfo(entity));
                }
            }
            else
            {
                this.GetOrAddCacheableItem(operationContext, this.CreateEntityInfo(data));
            }
        }
    }
}