// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MailKitImapConnection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Connectivity;
using Kephas.Connectivity.AttributedModel;
using Kephas.Cryptography;

namespace Kephas.Mail.Connectivity;

/// <summary>
/// Connection factory for <see cref="MailKitImapConnection"/>.
/// </summary>
/// <seealso cref="IConnectionFactory" />
[ConnectionKind("imap")]
public class MailKitImapConnectionFactory : IConnectionFactory
{
    private readonly IEncryptionService encryptionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MailKitImapConnectionFactory"/> class.
    /// </summary>
    /// <param name="encryptionService">The encryption service.</param>
    public MailKitImapConnectionFactory(IEncryptionService encryptionService)
    {
        this.encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
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
        return new MailKitImapConnection(context, this.encryptionService);
    }
}
