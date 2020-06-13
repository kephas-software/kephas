// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoDataContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the mongo data context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Runtime;

namespace Kephas.Data.MongoDB
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;

    using global::MongoDB.Driver;

    using Kephas.Composition;
    using Kephas.Data.Behaviors;
    using Kephas.Data.Commands.Factory;
    using Kephas.Data.MongoDB.Diagnostics;
    using Kephas.Data.MongoDB.Linq;
    using Kephas.Data.MongoDB.Resources;
    using Kephas.Data.Store;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A data context for MongoDB.
    /// </summary>
    [SupportedDataStoreKinds(DataStoreKind.MongoDB)]
    public class MongoDataContext : DataContextBase
    {
        private static readonly ConcurrentDictionary<string, MongoClient> MongoClients =
            new ConcurrentDictionary<string, MongoClient>();

        private readonly IRuntimeTypeRegistry typeRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDataContext"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="dataCommandProvider">The data command provider.</param>
        /// <param name="dataBehaviorProvider">The data behavior provider.</param>
        /// <param name="typeRegistry">The type registry.</param>
        public MongoDataContext(
            ICompositionContext compositionContext,
            IDataCommandProvider dataCommandProvider,
            IDataBehaviorProvider dataBehaviorProvider,
            IRuntimeTypeRegistry typeRegistry)
            : base(compositionContext, dataCommandProvider, dataBehaviorProvider)
        {
            this.typeRegistry = typeRegistry;
            Requires.NotNull(compositionContext, nameof(compositionContext));
            Requires.NotNull(dataCommandProvider, nameof(dataCommandProvider));
            Requires.NotNull(dataBehaviorProvider, nameof(dataBehaviorProvider));
        }

        /// <summary>
        /// Gets the MongoDB client.
        /// </summary>
        /// <value>
        /// The MongoDB client.
        /// </value>
        public IMongoClient Client { get; private set; }

        /// <summary>
        /// Gets the MongoDB database.
        /// </summary>
        /// <value>
        /// The MongoDB database.
        /// </value>
        public IMongoDatabase Database { get; private set; }

        /// <summary>
        /// Gets the collection name for the provided entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>
        /// The collection name.
        /// </returns>
        protected internal virtual string GetCollectionName(Type entityType)
        {
            return entityType.Name;
        }

        /// <summary>
        /// Gets a query over the entity type for the given query operation context, if any is provided.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="queryOperationContext">Context for the query.</param>
        /// <returns>
        /// A query over the entity type.
        /// </returns>
        protected override IQueryable<T> QueryCore<T>(IQueryOperationContext queryOperationContext)
        {
            var nativeQuery = this.Database.GetCollection<T>(this.GetCollectionName(typeof(T))).AsQueryable();
            var provider = new MongoQueryProvider(queryOperationContext, nativeQuery.Provider, this.typeRegistry);
            var query = new MongoQuery<T>(provider, nativeQuery, this.typeRegistry);
            return query;
        }

        /// <summary>
        /// Initializes the <see cref="MongoDataContext"/>.
        /// </summary>
        /// <param name="dataInitializationContext">The data initialization context.</param>
        protected override void Initialize(IDataInitializationContext dataInitializationContext)
        {
            Requires.NotNull(dataInitializationContext.DataStore, nameof(dataInitializationContext.DataStore));
            Requires.NotNull(dataInitializationContext.DataStore.DataContextSettings, nameof(dataInitializationContext.DataStore.DataContextSettings));

            base.Initialize(dataInitializationContext);

            var config = dataInitializationContext.DataStore.DataContextSettings;
            var mongoUrl = MongoUrl.Create(config.ConnectionString);
            var databaseName = mongoUrl.DatabaseName;
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new MongoDataException(Strings.Initialize_DatabaseNameEmpty_Exception);
            }

            this.Client = this.GetOrCreateMongoClient(mongoUrl);
            this.Database = this.Client.GetDatabase(databaseName);
        }

        /// <summary>
        /// Gets the mongo client, if one is available for the provided URL, otherwise creates one and caches it.
        /// </summary>
        /// <param name="mongoUrl">The Mongo URL.</param>
        /// <returns>A mongo client.</returns>
        private MongoClient GetOrCreateMongoClient(MongoUrl mongoUrl)
        {
            // see https://www.mongodb.com/blog/post/introducing-20-net-driver
            // "The typical pattern is for an application to *create a single MongoClient instance*,
            //  call GetDatabase to get a IMongoDatabase instance, and finally call GetCollection
            //  on the database object to get a IMongoCollection instance."
            //
            // see also http://blog.mongolab.com/2013/11/deep-dive-into-connection-pooling/
            // "Here the mongoClient object holds your connection pool, and will give your app connections as needed.
            //  You should strive to create this object once as your application initializes and re-use this object
            //  throughout your application to talk to your database. The most common connection pooling problem we see
            //  results from applications that create a MongoClient object way too often, sometimes on each database request.
            //  If you do this you will not be using your connection pool as each MongoClient object maintains a separate
            //  pool that is not being reused by your application."
            return MongoClients.GetOrAdd(
              mongoUrl.ToString(),
              _ =>
              {
                  var settings = MongoClientSettings.FromUrl(mongoUrl);

                  // see http://docs.mongolab.com/connecting/#known-issues
                  // "The most effective workaround we’ve found in working with Azure and our customers
                  //  has been to set the max connection idle time below four minutes. The idea is to make
                  //  the driver recycle idle connections before the firewall forces the issue.
                  //  For example, one customer, who is using the C# driver, set MongoDefaults.MaxConnectionIdleTime
                  //  to one minute and it cleared up the issue."
                  settings.MaxConnectionIdleTime = TimeSpan.FromMinutes(1);
                  settings.MaxConnectionPoolSize = 1000;
                  settings.ClusterConfigurator = b =>
                  {
                      b.ConfigureConnectionPool(s => s.With());
                      b.Subscribe(new MongoDataContextLogEventSubscriber(this.CompositionContext));
                  };

                  return new MongoClient(settings);
              });
        }
    }
}