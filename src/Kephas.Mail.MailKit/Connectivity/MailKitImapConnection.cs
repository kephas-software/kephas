﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MailKitImapConnection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Connectivity;

using Kephas.Connectivity;
using Kephas.Cryptography;
using Kephas.Security.Authentication;
using Kephas.Threading.Tasks;
using MailKit.Net.Imap;

/// <summary>
/// Provides an IMAP connection to a mail server.
/// </summary>
/// <seealso cref="IConnection" />
public class MailKitImapConnection : IConnection, IAdapter<ImapClient>
{
    private readonly IConnectionContext connectionContext;
    private readonly IEncryptionService encryptionService;
    private readonly ImapClient imapClient;
    private bool disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="MailKitImapConnection" /> class.
    /// </summary>
    /// <param name="connectionContext">The connection context.</param>
    /// <param name="encryptionService">The encryption service.</param>
    public MailKitImapConnection(IConnectionContext connectionContext, IEncryptionService encryptionService)
    {
        this.connectionContext = connectionContext;
        this.encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
        this.imapClient = new ImapClient
        {
            ServerCertificateValidationCallback = (s, c, h, e) => true,
        };
    }

    /// <summary>
    /// Gets the object the current instance adapts.
    /// </summary>
    /// <value>
    /// The object the current instance adapts.
    /// </value>
    ImapClient IAdapter<ImapClient>.Of => this.imapClient;

    /// <summary>
    /// Opens the connection asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The asynchronous result.
    /// </returns>
    public virtual async Task OpenAsync(CancellationToken cancellationToken = default)
    {
        var context = this.connectionContext;
        if (context.Host is null)
        {
            throw new ArgumentException($"The host is not provided. Please provide a host URI as: imap://serverName:port.", nameof(context.Host));
        }

        var clearTextCredentials = context.Credentials switch
        {
            IUserClearTextPasswordCredentials credentials => credentials,
            IUserPasswordCredentials credentials => new UserClearTextPasswordCredentials(credentials.UserName, this.encryptionService.Decrypt(credentials.Password)),
            _ => throw new ArgumentException($"The credentials are not provided or are not of any of the types: '{typeof(UserPasswordCredentials)}', '{typeof(UserClearTextPasswordCredentials)}'.", nameof(context.Credentials)),
        };

        var port = context.Host.IsDefaultPort ? 993 : context.Host.Port;
        var serverName = context.Host.Host;

        await this.imapClient.ConnectAsync(serverName, port, useSsl: true, cancellationToken).PreserveThreadContext();
        await this.imapClient.AuthenticateAsync(clearTextCredentials.UserName, clearTextCredentials.ClearTextPassword, cancellationToken);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (this.disposedValue)
        {
            return;
        }

        if (disposing)
        {
            this.imapClient.Dispose();
        }

        this.disposedValue = true;
    }
}
