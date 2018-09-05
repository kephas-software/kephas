// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MailKitEmailSenderService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the mail kit email sender service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Services
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Logging;
    using Kephas.Mail.Configuration;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    using MailKit.Net.Smtp;
    using MailKit.Security;

    using MimeKit;

    /// <summary>
    /// A MailKit email sender service.
    /// </summary>
    public abstract class MailKitEmailSenderServiceBase : IEmailSenderService
    {
        /// <summary>
        /// Information describing the connection (cached).
        /// </summary>
        private (ICredentials credentials, string host, int port)? connectionData;

        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="emailMessage">The email message.</param>
        /// <param name="context">The sending context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An asynchronous result.</returns>
        public async Task SendAsync(
            IEmailMessage emailMessage,
            IContext context = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using (var smtpClient = await this.GetSmtpClientAsync(context, cancellationToken).PreserveThreadContext())
                {
                    var nativeMessage = this.GetNormalizedMailMessage(emailMessage);
                    await smtpClient.SendAsync(nativeMessage, cancellationToken).PreserveThreadContext();
                }
            }
            catch (Exception ex)
            {
                // TODO localization
                this.Logger.Error(ex, $"Error while sending mail.");
                throw;
            }
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<MailKitEmailSenderServiceBase> Logger { get; set; }

        /// <summary>
        /// Creates email message builder.
        /// </summary>
        /// <param name="context">The sending context.</param>
        /// <returns>
        /// The new email message builder.
        /// </returns>
        public IEmailMessageBuilder CreateEmailMessageBuilder(IContext context = null)
        {
            return new MailKitEmailMessageBuilder();
        }

        /// <summary>
        /// Gets the email sender settings.
        /// </summary>
        /// <returns>
        /// The email sender settings.
        /// </returns>
        protected abstract SmtpSettings GetEmailSenderSettings();

        /// <summary>
        /// Gets normalized mail message.
        /// </summary>
        /// <param name="emailMessage">The email message.</param>
        /// <returns>
        /// The normalized mail message.
        /// </returns>
        protected virtual MimeMessage GetNormalizedMailMessage(IEmailMessage emailMessage)
        {
            var nativeMessage = (MimeMessage)emailMessage;
            var settings = this.GetEmailSenderSettings();
            nativeMessage.Sender = new MailboxAddress(settings.UserName);

            // if no recipient provided, the mail is sent to the configured sys admin.
            if (nativeMessage.To.Count == 0 && nativeMessage.Cc.Count == 0 && nativeMessage.Bcc.Count == 0)
            {
                nativeMessage.To.Add(new MailboxAddress(settings.UserName));
            }

            return nativeMessage;
        }

        /// <summary>
        /// Creates SMTP client.
        /// </summary>
        /// <param name="context">The sending context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The new SMTP client as promise.
        /// </returns>
        protected virtual async Task<SmtpClient> GetSmtpClientAsync(IContext context, CancellationToken cancellationToken)
        {
            var smtpClient = new SmtpClient();

            (var credentials, var host, var port) = this.GetConnectionData(context);
            await smtpClient.AuthenticateAsync(credentials, cancellationToken).PreserveThreadContext();
            await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTlsWhenAvailable, cancellationToken).PreserveThreadContext();

            return smtpClient;
        }

        /// <summary>
        /// Gets the connection data.
        /// </summary>
        /// <param name="context">The sending context.</param>
        /// <returns>
        /// The connection data.
        /// </returns>
        protected virtual (ICredentials credentials, string host, int port) GetConnectionData(IContext context)
        {
            return this.connectionData ?? (this.connectionData = this.ComputeConnectionData()).Value;
        }

        /// <summary>
        /// Calculates the connection data.
        /// </summary>
        /// <returns>
        /// The calculated connection data.
        /// </returns>
        private (NetworkCredential, string Host, int) ComputeConnectionData()
        {
            var settings = this.GetEmailSenderSettings();
            return (new NetworkCredential(settings.UserName, settings.Password), settings.Host, settings.Port ?? SmtpSettings.DefaultSmtpPort);
        }
    }
}