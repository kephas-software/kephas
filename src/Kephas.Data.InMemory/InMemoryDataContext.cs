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
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands.Factory;
    using Kephas.Data.Store;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Serialization;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Client data context managing.
    /// </summary>
    /// <remarks>
    /// For the connection string, use something like in the example below:
    /// ConnectionString=UseSharedCache=true|false;Data=(JSON serialized data).
    /// </remarks>
    [SupportedDataStoreKinds(DataStoreKind.InMemory)]
    public class InMemoryDataContext : DataContextBase
    {
        /// <summary>
        /// The shared cache.
        /// </summary>
        private static readonly ConcurrentBag<object> SharedCache = new ConcurrentBag<object>();

        /// <summary>
        /// The internal cache.
        /// </summary>
        private readonly List<object> cache = new List<object>();

        /// <summary>
        /// The working cache.
        /// </summary>
        private IEnumerable<object> workingCache;

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
        /// Gets a value indicating whether to use the shared cache or not.
        /// </summary>
        /// <value>
        /// True if shared cache is used, false if not.
        /// </value>
        protected bool UseSharedCache { get; private set; }

        /// <summary>
        /// Gets the working cache.
        /// </summary>
        /// <value>
        /// The working cache.
        /// </value>
        protected IEnumerable<object> WorkingCache
        {
            get
            {
                this.InitializationMonitor.AssertIsCompletedSuccessfully();
                return this.workingCache;
            }
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
            return this.WorkingCache.OfType<T>().AsQueryable();
        }

        /// <summary>
        /// Gets or add a cacheable item.
        /// </summary>
        /// <param name="operationContext">Context for the operation.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="changeState">The entity change state.</param>
        /// <returns>
        /// The or add cached item.
        /// </returns>
        internal object GetOrAddCacheableItem(IDataOperationContext operationContext, object entity, ChangeState changeState)
        {
            Requires.NotNull(entity, nameof(entity));

            if (changeState == ChangeState.Added || changeState == ChangeState.AddedOrChanged)
            {
                this.Add(entity);
                return entity;
            }

            var identifiable = this.TryGetCapability<IIdentifiable>(entity, operationContext);
            var entityId = identifiable?.Id;
            if (identifiable == null || Id.IsUnsetValue(entityId))
            {
                this.Add(entity);
                return entity;
            }

            var entityType = entity.GetType();
            var existingEntity = this.WorkingCache.FirstOrDefault(e => e.GetType() == entityType && entityId.Equals(this.TryGetCapability<IIdentifiable>(e, operationContext)?.Id));
            if (existingEntity != null)
            {
                return existingEntity;
            }

            this.Add(entity);
            return entity;
        }

        /// <summary>
        /// Initializes the core.
        /// </summary>
        /// <param name="config">The configuration.</param>
        protected override void InitializeCore(IDataContextConfiguration config)
        {
            var connectionStringValues = string.IsNullOrWhiteSpace(config?.ConnectionString)
                                             ? new Dictionary<string, string>()
                                             : ConnectionStringParser.Parse(config.ConnectionString);

            bool.TryParse(connectionStringValues.TryGetValue("UseSharedCache"), out var useSharedCache);
            this.UseSharedCache = useSharedCache;
            this.workingCache = useSharedCache ? (IEnumerable<object>)SharedCache : this.cache;

            var serializedData = connectionStringValues.TryGetValue("Data");
            this.InitializeData(serializedData);
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
                this.cache.Clear();
            }
        }

        /// <summary>
        /// Adds an item to the internal cache.
        /// </summary>
        /// <param name="item">The item to add.</param>
        private void Add(object item)
        {
            if (this.UseSharedCache)
            {
                SharedCache.Add(item);
            }
            else
            {
                this.cache.Add(item);
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
            if (data is IEnumerable)
            {
                foreach (var entity in (IEnumerable)data)
                {
                    this.GetOrAddCacheableItem(operationContext, entity, ChangeState.NotChanged);
                }
            }
            else
            {
                this.GetOrAddCacheableItem(operationContext, data, ChangeState.NotChanged);
            }
        }
    }
}