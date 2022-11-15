// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoClientSettingsProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.MongoDB;

using System.Security.Authentication;

using global::MongoDB.Driver;
using Kephas.Services;
using Kephas.MongoDB.Diagnostics;
using Kephas.Services;

/// <summary>
/// The default implementation of <see cref="IMongoClientSettingsProvider"/>.
/// </summary>
[OverridePriority(Priority.Low)]
public class MongoClientSettingsProvider : IMongoClientSettingsProvider
{
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoClientSettingsProvider"/> class.
    /// </summary>
    /// <param name="serviceProvider">The injector.</param>
    public MongoClientSettingsProvider(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Gets the client settings for the provided MongoDB URL.
    /// </summary>
    /// <param name="mongoUrl">The URL.</param>
    /// <returns>The MongoDB settings.</returns>
    public MongoClientSettings GetClientSettings(MongoUrl mongoUrl)
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
            b.Subscribe(new MongoLogEventSubscriber(this.serviceProvider));
        };
        settings.SslSettings = new SslSettings
        {
            EnabledSslProtocols = SslProtocols.Tls12,
        };

        return settings;
    }
}