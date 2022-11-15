// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExchangeOnlineImapConnectionFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Connectivity;

using Kephas.Configuration;
using Kephas.Connectivity;
using Kephas.Connectivity.AttributedModel;
using Kephas.Cryptography;
using Kephas.Logging;
using Kephas.Serialization;

/// <summary>
/// Connection factory for <see cref="ExchangeOnlineImapConnection"/>.
/// </summary>
/// <seealso cref="IConnectionFactory" />
[ConnectionKind(ConnectionKind)]
public class ExchangeOnlineImapConnectionFactory : IConnectionFactory
{
    /// <summary>
    /// The 'imap' connection kind.
    /// </summary>
    public const string ConnectionKind = "imap-ms365";

    private readonly IEncryptionService encryptionService;
    private readonly IConfiguration<ExchangeOnlineConnectionSettings> connectionConfiguration;
    private readonly ILogger<ExchangeOnlineImapConnection>? logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExchangeOnlineImapConnectionFactory"/> class.
    /// </summary>
    /// <param name="encryptionService">The encryption service.</param>
    /// <param name="connectionConfiguration">The connection configuration.</param>
    /// <param name="logger">Optional. The logger.</param>
    public ExchangeOnlineImapConnectionFactory(
        IEncryptionService encryptionService,
        IConfiguration<ExchangeOnlineConnectionSettings> connectionConfiguration,
        ILogger<ExchangeOnlineImapConnection>? logger = null)
    {
        this.encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
        this.connectionConfiguration = connectionConfiguration ?? throw new ArgumentNullException(nameof(connectionConfiguration));
        this.logger = logger;
    }

    /// <summary>
    /// Creates the connection configured through the connection context.
    /// </summary>
    /// <param name="context">The connection creation context.</param>
    /// <returns>
    /// The newly created connection.
    /// </returns>
    public IConnection CreateConnection(IConnectionContext context)
    {
        return new ExchangeOnlineImapConnection(context, this.encryptionService, this.connectionConfiguration, this.logger);
    }
}
