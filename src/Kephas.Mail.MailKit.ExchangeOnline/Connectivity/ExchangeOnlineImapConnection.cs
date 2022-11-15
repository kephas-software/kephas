// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MailKitImapConnection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Connectivity;

using Kephas.Configuration;
using Kephas.Connectivity;
using Kephas.Cryptography;
using Kephas.Logging;
using Kephas.Security.Authentication;
using Kephas.Serialization;
using Kephas.Threading.Tasks;
using MailKit.Net.Imap;
using MailKit.Security;
using Microsoft.Identity.Client;

/// <summary>
/// Provides an IMAP connection to a mail server.
/// </summary>
/// <seealso cref="IConnection" />
public class ExchangeOnlineImapConnection : IConnection, IAdapter<ImapClient>
{
    private readonly IConnectionContext connectionContext;
    private readonly IEncryptionService encryptionService;
    private readonly IConfiguration<ExchangeOnlineConnectionSettings> connectionConfiguration;
    private readonly ILogger<ExchangeOnlineImapConnection>? logger;
    private readonly ImapClient imapClient;
    private bool disposedValue;
    private bool isDisposing;
    private Timer? refreshTimer;
    private Task? refreshTask;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExchangeOnlineImapConnection" /> class.
    /// </summary>
    /// <param name="connectionContext">The connection context.</param>
    /// <param name="encryptionService">The encryption service.</param>
    /// <param name="connectionConfiguration">The connection configuration.</param>
    /// <param name="logger">Optional. The logger.</param>
    public ExchangeOnlineImapConnection(
        IConnectionContext connectionContext,
        IEncryptionService encryptionService,
        IConfiguration<ExchangeOnlineConnectionSettings> connectionConfiguration,
        ILogger<ExchangeOnlineImapConnection>? logger = null)
    {
        this.connectionContext = connectionContext;
        this.encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
        this.connectionConfiguration = connectionConfiguration ?? throw new ArgumentNullException(nameof(connectionConfiguration));
        this.logger = logger;
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
        // check hints here
        // https://stackoverflow.com/questions/72984691/how-to-use-mailkit-with-imap-for-exchange-with-oauth2-for-daemon-non-interacti
        var context = this.connectionContext;
        if (context.Host is null)
        {
            throw new InvalidOperationException($"The host is not provided. Please provide a host URI as: imap://serverName:port.");
        }

        var clearTextCredentials = context.Credentials switch
        {
            IUserClearTextPasswordCredentials credentials => credentials,
            IUserPasswordCredentials credentials => new UserClearTextPasswordCredentials(credentials.UserName, this.encryptionService.Decrypt(credentials.Password)),
            _ => throw new InvalidOperationException($"The credentials are not provided or are not of any of the types: '{typeof(UserPasswordCredentials)}', '{typeof(UserClearTextPasswordCredentials)}'."),
        };

        var port = context.Host.IsDefaultPort ? 993 : context.Host.Port;
        var serverName = context.Host.Host;

        var saslMechanism = this.GetSaslMechanism(
            await this.AuthenticateAsync(clearTextCredentials, cancellationToken).PreserveThreadContext());

        await this.imapClient.ConnectAsync(serverName, port, useSsl: true, cancellationToken).PreserveThreadContext();
        await this.imapClient.AuthenticateAsync(saslMechanism, cancellationToken).PreserveThreadContext();
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    public void Dispose()
    {
        this.isDisposing = true;

        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);

        this.isDisposing = false;
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
            lock (this.imapClient)
            {
                this.refreshTimer?.Dispose();
            }
        }

        this.disposedValue = true;
    }

    private async Task<AuthenticationResult> AuthenticateAsync(IUserClearTextPasswordCredentials credentials, CancellationToken cancellationToken)
    {
        // https://github.com/jstedfast/MailKit/blob/master/ExchangeOAuth2.md
        var settings = this.connectionConfiguration.GetSettings();
        var scopes = settings.Scopes;
        if (scopes is null || scopes.Count == 0)
        {
            throw new InvalidOperationException("At least one scope is required.");
        }

        var clientId = settings.ClientId ?? throw new InvalidOperationException("The client ID is not provided.");
        var options = new PublicClientApplicationOptions
        {
            TenantId = settings.TenantId,
            ClientId = clientId,
        };

        var app = PublicClientApplicationBuilder
            .CreateWithApplicationOptions(options)
            .Build();
        var authResult = await app.AcquireTokenByUsernamePassword(scopes, credentials.UserName, credentials.ClearTextPassword).ExecuteAsync(cancellationToken).PreserveThreadContext();

        this.EnqueueRefresh(app, authResult, TimeSpan.FromSeconds(settings.RefreshTokenSecondsBeforeExpiration));

        return authResult;
    }

    private void EnqueueRefresh(IPublicClientApplication app, AuthenticationResult authResult, TimeSpan timeBeforeExpiration)
    {
        var dueTime = authResult.ExpiresOn - DateTimeOffset.Now - timeBeforeExpiration; // TimeSpan.FromSeconds(3);
        lock (this.imapClient)
        {
            if (this.isDisposing || this.disposedValue)
            {
                return;
            }

            this.refreshTimer ??= new Timer(
                _ =>
                {
                    this.refreshTask = app.AcquireTokenSilent(authResult.Scopes, authResult.Account)
                        .ExecuteAsync()
                        .ContinueWith(t =>
                        {
                            if (t.IsFaulted)
                            {
                                this.logger.Error(t.Exception, "Errors during refreshing the token for {user}.", authResult.Account.Username);
                            }
                            else if (t.IsCanceled)
                            {
                                this.logger.Warn("Refreshing the token for {user} was canceled.", authResult.Account.Username);
                            }
                            else
                            {
                                this.EnqueueRefresh(app, t.Result, timeBeforeExpiration);
                                this.logger.Info("Successfully refreshed the token for {user}.", authResult.Account.Username);
                            }
                        });
                },
                null,
                dueTime,
                Timeout.InfiniteTimeSpan);
            this.refreshTimer?.Change(dueTime, Timeout.InfiniteTimeSpan);
        }
    }

    private SaslMechanismOAuth2 GetSaslMechanism(AuthenticationResult authResult)
    {
        return new SaslMechanismOAuth2(authResult.Account.Username, authResult.AccessToken);
    }
}