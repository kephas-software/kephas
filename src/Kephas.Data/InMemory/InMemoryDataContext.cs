// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryDataContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the in memory data context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.InMemory
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Composition;
    using Kephas.Data.Behaviors;
    using Kephas.Data.Caching;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands.Factory;
    using Kephas.Data.InMemory.Caching;
    using Kephas.Data.Store;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;
    using Kephas.Serialization;

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
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="dataCommandProvider">The data command provider.</param>
        /// <param name="dataBehaviorProvider">The data behavior provider.</param>
        /// <param name="serializationService">The serialization service.</param>
        public InMemoryDataContext(ICompositionContext compositionContext, IDataCommandProvider dataCommandProvider, IDataBehaviorProvider dataBehaviorProvider, ISerializationService serializationService)
            : base(compositionContext, dataCommandProvider, dataBehaviorProvider)
        {
            Requires.NotNull(compositionContext, nameof(compositionContext));
            Requires.NotNull(dataCommandProvider, nameof(dataCommandProvider));
            Requires.NotNull(dataBehaviorProvider, nameof(dataBehaviorProvider));
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
        /// Gets the local cache where the session entities are stored.
        /// </summary>
        /// <value>
        /// The local cache.
        /// </value>
        protected internal override IDataContextCache LocalCache
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
        /// Gets a query over the entity type for the given query operationContext, if any is provided.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="queryOperationContext">Context for the query.</param>
        /// <returns>
        /// A query over the entity type.
        /// </returns>
        protected override IQueryable<T> QueryCore<T>(IQueryOperationContext queryOperationContext)
        {
            return this.LocalCache.Values.Select(ei => ei.Entity).OfType<T>().AsQueryable();
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
            base.Initialize(dataInitializationContext);

            var config = dataInitializationContext?.DataStore.DataContextConfiguration;
            var connectionStringValues = string.IsNullOrWhiteSpace(config?.ConnectionString)
                                             ? new Dictionary<string, string>()
                                             : ConnectionStringParser.AsDictionary(config.ConnectionString);

            this.InitializeLocalCache(config as InMemoryDataContextConfiguration, connectionStringValues);

            this.InitializeData(config as InMemoryDataContextConfiguration, connectionStringValues, dataInitializationContext);
        }

        /// <summary>
        /// Initializes the data.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="connectionStringValues">The connection string values.</param>
        /// <param name="dataInitializationContext">The data initialization context.</param>
        private void InitializeData(InMemoryDataContextConfiguration config, IDictionary<string, string> connectionStringValues, IDataInitializationContext dataInitializationContext)
        {
            var serializedData = connectionStringValues.TryGetValue(nameof(InMemoryDataContextConfiguration.InitialData));
            this.InitializeData(serializedData);

            this.InitializeData(config?.InitialData);
            this.InitializeData(dataInitializationContext?.InitializationContext?.InitialData());
        }

        /// <summary>
        /// Initializes the local cache possibly using the shared cache if so configured.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="connectionStringValues">The connection string values.</param>
        private void InitializeLocalCache(
            InMemoryDataContextConfiguration config,
            IDictionary<string, string> connectionStringValues)
        {
            bool useSharedCache;
            if (config?.UseSharedCache.HasValue ?? false)
            {
                useSharedCache = config.UseSharedCache.Value;
            }
            else
            {
                bool.TryParse(
                    connectionStringValues.TryGetValue(nameof(InMemoryDataContextConfiguration.UseSharedCache)),
                    out useSharedCache);
            }

            this.UseSharedCache = useSharedCache;
            this.workingCache = useSharedCache ? SharedCache : base.LocalCache;
        }

        /// <summary>
        /// Initializes the data.
        /// </summary>
        /// <param name="initialData">The initial data.</param>
        private void InitializeData(IEnumerable<IChangeStateTrackableEntityInfo> initialData)
        {
            if (initialData == null)
            {
                return;
            }

            foreach (var entityInfo in initialData)
            {
                var ownEntityInfo = this.CreateEntityInfo(entityInfo.Entity, entityInfo.ChangeState);
                this.workingCache[ownEntityInfo.Id] = ownEntityInfo;
            }
        }

        /// <summary>
        /// Initializes the data from the provided connection string.
        /// </summary>
        /// <param name="serializedInitialData">The serialized initial data.</param>
        private void InitializeData(string serializedInitialData)
        {
            if (string.IsNullOrWhiteSpace(serializedInitialData))
            {
                return;
            }

            var data = this.SerializationService.JsonDeserialize(serializedInitialData);

            if (data.GetType().IsCollection())
            {
                foreach (var entity in (IEnumerable)data)
                {
                    var entityInfo = this.CreateEntityInfo(entity);
                    this.workingCache.Add(entityInfo);
                }
            }
            else
            {
                var entityInfo = this.CreateEntityInfo(data);
                this.workingCache.Add(entityInfo);
            }
        }
    }
}