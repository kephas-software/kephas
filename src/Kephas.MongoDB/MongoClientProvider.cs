// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoClientProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.MongoDB;

using System.Collections.Concurrent;
using global::MongoDB.Bson.Serialization;
using global::MongoDB.Bson.Serialization.Conventions;
using global::MongoDB.Driver;
using Kephas.Configuration;
using Kephas.Logging;
using Kephas.MongoDB.Conventions;
using Kephas.MongoDB.Serializers;
using Kephas.Services;

/// <summary>
/// The default implementation of the <see cref="IMongoClientProvider"/>.
/// </summary>
[OverridePriority(Priority.Low)]
public class MongoClientProvider : Loggable, IMongoClientProvider
{
    private static readonly object Sync = new ();
    private static bool isDriverInitialized = false;

    private readonly IMongoClientSettingsProvider clientSettingsProvider;
    private readonly ICollection<Lazy<IMongoSerializer, MongoSerializerMetadata>> serializers;
    private readonly ICollection<Lazy<IMongoConventionsProvider, AppServiceMetadata>> conventionsProviders;
    private readonly IConfiguration<MongoSettings>? mongoConfiguration;
    private readonly ConcurrentDictionary<MongoUrl, IMongoClient> mongoClients = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoClientProvider"/> class.
    /// </summary>
    /// <param name="clientSettingsProvider">The client settings provider.</param>
    /// <param name="serializers">The serializers.</param>
    /// <param name="conventionsProviders">The conventions providers.</param>
    /// <param name="mongoConfiguration">Optional. The MongoDB connection configuration.</param>
    /// <param name="logManager">Optional. The log manager.</param>
    public MongoClientProvider(
        IMongoClientSettingsProvider clientSettingsProvider,
        ICollection<Lazy<IMongoSerializer, MongoSerializerMetadata>> serializers,
        ICollection<Lazy<IMongoConventionsProvider, AppServiceMetadata>> conventionsProviders,
        IConfiguration<MongoSettings>? mongoConfiguration = null,
        ILogManager? logManager = null)
        : base(logManager)
    {
        this.clientSettingsProvider =
            clientSettingsProvider ?? throw new ArgumentNullException(nameof(clientSettingsProvider));
        this.serializers = serializers ?? throw new ArgumentNullException(nameof(serializers));
        this.conventionsProviders = conventionsProviders ?? throw new ArgumentNullException(nameof(conventionsProviders));
        this.mongoConfiguration = mongoConfiguration;
    }

    /// <summary>
    /// Gets the configured <see cref="IMongoClient"/> instance.
    /// </summary>
    /// <param name="context">The mongo context.</param>
    /// <returns>The configured <see cref="IMongoClient"/> instance.</returns>
    public IMongoClient GetMongoClient(IMongoContext? context = null)
    {
        var connectionString =
            context?.ConnectionString ?? this.mongoConfiguration?.GetSettings(context).ConnectionString;
        if (connectionString is null or { Length: 0 })
        {
            throw new InvalidOperationException(
                "The provided context does not contain a connection string, not the configuration.");
        }

        this.EnsureDriverInitialized();

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
        var mongoUrl = new MongoUrl(connectionString);
        return this.mongoClients.GetOrAdd(
            mongoUrl,
            _ =>
            {
                var clientSettings = this.clientSettingsProvider.GetClientSettings(mongoUrl);
                return new MongoClient(clientSettings);
            });
    }

    private void EnsureDriverInitialized()
    {
        if (isDriverInitialized)
        {
            return;
        }

        lock (Sync)
        {
            if (isDriverInitialized)
            {
                return;
            }

            foreach (var conventionProvider in this.conventionsProviders.Order().Select(p => p.Value))
            {
                var (name, pack, filter) = conventionProvider.GetConventions();
                this.Logger?.Info("Registering {conventions}.", name);
                ConventionRegistry.Register(name, pack, filter);
            }

            foreach (var serializer in this.serializers.Order())
            {
                this.Logger?.Info("Registering {serializer}.", serializer.Value);
                BsonSerializer.RegisterSerializer(serializer.Metadata.ValueType, serializer.Value);
            }

            isDriverInitialized = true;
        }
    }
}