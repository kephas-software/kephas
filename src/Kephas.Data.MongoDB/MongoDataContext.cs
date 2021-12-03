// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoDataContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the mongo data context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Data.MongoDB
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Security.Authentication;

    using global::MongoDB.Driver;
    using Kephas.Data.Behaviors;
    using Kephas.Data.Commands.Factory;
    using Kephas.Data.MongoDB.Diagnostics;
    using Kephas.Data.MongoDB.Linq;
    using Kephas.Data.MongoDB.Resources;
    using Kephas.Data.Store;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Runtime;

    /// <summary>
    /// A data context for MongoDB.
    /// </summary>
    [SupportedDataStoreKinds(DataStoreKind.MongoDB)]
    public class MongoDataContext : DataContextBase
    {
        private static readonly ConcurrentDictionary<string, MongoClient> MongoClients = new();

        private readonly IRuntimeTypeRegistry typeRegistry;
        private readonly IMongoNamingStrategy namingStrategy;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDataContext"/> class.
        /// </summary>
        /// <param name="injector">The injector.</param>
        /// <param name="dataCommandProvider">The data command provider.</param>
        /// <param name="dataBehaviorProvider">The data behavior provider.</param>
        /// <param name="typeRegistry">The type registry.</param>
        /// <param name="namingStrategy">The naming strategy.</param>
        public MongoDataContext(
            IInjector injector,
            IDataCommandProvider dataCommandProvider,
            IDataBehaviorProvider dataBehaviorProvider,
            IRuntimeTypeRegistry typeRegistry,
            IMongoNamingStrategy namingStrategy)
            : base(injector, dataCommandProvider, dataBehaviorProvider)
        {
            injector = injector ?? throw new ArgumentNullException(nameof(injector));
            dataCommandProvider = dataCommandProvider ?? throw new System.ArgumentNullException(nameof(dataCommandProvider));
            dataBehaviorProvider = dataBehaviorProvider ?? throw new System.ArgumentNullException(nameof(dataBehaviorProvider));
            typeRegistry = typeRegistry ?? throw new ArgumentNullException(nameof(typeRegistry));
            namingStrategy = namingStrategy ?? throw new System.ArgumentNullException(nameof(namingStrategy));

            this.typeRegistry = typeRegistry;
            this.namingStrategy = namingStrategy;
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
        /// Gets a query over the entity type for the given query operation context, if any is provided.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="queryOperationContext">Context for the query.</param>
        /// <returns>
        /// A query over the entity type.
        /// </returns>
        protected override IQueryable<T> QueryCore<T>(IQueryOperationContext queryOperationContext)
        {
            var nativeQuery = this.Database.GetCollection<T>(
                this.namingStrategy.GetCollectionName(this, typeof(T)))
                .AsQueryable();
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
            if (dataInitializationContext.DataStore == null)
            {
                throw new ArgumentException("The data store is not set in the initialization context.", nameof(dataInitializationContext));
            }

            if (dataInitializationContext.DataStore.DataContextSettings == null)
            {
                throw new ArgumentException("The data context settings are not set in the data store from the initialization context.", nameof(dataInitializationContext));
            }

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
        protected virtual MongoClient GetOrCreateMongoClient(MongoUrl mongoUrl)
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
                _ => new MongoClient(this.GetClientSettings(mongoUrl)));
        }

        /// <summary>
        /// Gets the client settings for the provided MongoDB URL.
        /// </summary>
        /// <param name="mongoUrl">The URL.</param>
        /// <returns>The MongoDB settings.</returns>
        protected virtual MongoClientSettings GetClientSettings(MongoUrl mongoUrl)
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
                b.Subscribe(new MongoDataContextLogEventSubscriber(this.Injector));
            };
            settings.SslSettings = new SslSettings
            {
                EnabledSslProtocols = SslProtocols.Tls12,
            };

            return settings;
        }
    }
}