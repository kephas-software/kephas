// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoClientProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.MongoDB;

using global::MongoDB.Driver;
using Kephas.Configuration;
using Kephas.Logging;
using Kephas.Services;

/// <summary>
/// The default implementation of the <see cref="IMongoClientProvider"/>.
/// </summary>
[OverridePriority(Priority.Low)]
public class MongoClientProvider : Loggable, IMongoClientProvider
{
    private readonly IMongoClientSettingsProvider clientSettingsProvider;
    private readonly IConfiguration<MongoSettings>? mongoConfiguration;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoClientProvider"/> class.
    /// </summary>
    /// <param name="clientSettingsProvider">The client settings provider.</param>
    /// <param name="mongoConfiguration">Optional. The MongoDB connection configuration.</param>
    /// <param name="logManager">Optional. The log manager.</param>
    public MongoClientProvider(IMongoClientSettingsProvider clientSettingsProvider, IConfiguration<MongoSettings>? mongoConfiguration = null, ILogManager? logManager = null)
        : base(logManager)
    {
        this.clientSettingsProvider = clientSettingsProvider ?? throw new ArgumentNullException(nameof(clientSettingsProvider));
        this.mongoConfiguration = mongoConfiguration;
    }

    /// <summary>
    /// Gets the configured <see cref="IMongoClient"/> instance.
    /// </summary>
    /// <param name="context">The mongo context.</param>
    /// <returns>The configured <see cref="IMongoClient"/> instance.</returns>
    public IMongoClient GetMongoClient(IMongoContext? context = null)
    {
        var connectionString = context?.ConnectionString ?? this.mongoConfiguration?.GetSettings(context).ConnectionString;
        if (connectionString is null or { Length: 0 })
        {
            throw new InvalidOperationException("The provided context does not contain a connection string, not the configuration.");
        }

        // TODO cache and add conventions and serializers.
        var mongoUrl = new MongoUrl(connectionString);
        var clientSettings = this.clientSettingsProvider.GetClientSettings(mongoUrl);
        return new MongoClient(clientSettings);
    }
}