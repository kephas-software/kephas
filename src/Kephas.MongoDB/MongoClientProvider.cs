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
    private readonly IOrderedLazyServiceCollection<IMongoSerializer, MongoSerializerMetadata> serializers;
    private readonly IOrderedLazyServiceCollection<IMongoConventionsProvider, AppServiceMetadata> conventionsProviders;
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
        IOrderedLazyServiceCollection<IMongoSerializer, MongoSerializerMetadata> serializers,
        IOrderedLazyServiceCollection<IMongoConventionsProvider, AppServiceMetadata> conventionsProviders,
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

            foreach (var conventionProvider in this.conventionsProviders
                         .OrderBy(p => p.Metadata.ProcessingPriority)
                         .Select(p => p.Value))
            {
                var (name, pack, filter) = conventionProvider.GetConventions();
                this.Logger?.Info("Registering {conventions}.", name);
                ConventionRegistry.Register(name, pack, filter);
            }

            foreach (var serializer in this.serializers.GetServiceFactories())
            {
                this.Logger?.Info("Registering {serializer}.", serializer.Value);
                BsonSerializer.RegisterSerializer(serializer.Metadata.ValueType, serializer.Value);
            }

            isDriverInitialized = true;
        }
    }
}