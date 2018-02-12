// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MailKitEmailSenderService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the mail kit email sender service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Services
{
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

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
        /// Information describing the connection.
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
            var smtpClient = new SmtpClient();

            (var credentials, var host, var port) = this.GetConnectionData(emailMessage);
            await smtpClient.AuthenticateAsync(credentials, cancellationToken).PreserveThreadContext();
            await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTlsWhenAvailable, cancellationToken).PreserveThreadContext();

            try
            {
                var mail = emailMessage.ToMailMessage();
                if (mail.Sender == null)
                {
                    var settings = this.GetEmailSenderSettings();
                    mail.Sender = new MailboxAddress(settings.UserName);
                }
                await smtpClient.SendAsync(mail, cancellationToken).PreserveThreadContext();
            }
            finally
            {
                await smtpClient.DisconnectAsync(quit: true, cancellationToken: cancellationToken).PreserveThreadContext();
            }
        }

        /// <summary>
        /// Gets the email sender settings.
        /// </summary>
        /// <returns>
        /// The email sender settings.
        /// </returns>
        protected abstract EmailSenderSettings GetEmailSenderSettings();

        /// <summary>
        /// Gets the connection data.
        /// </summary>
        /// <param name="emailMessage">The email message.</param>
        /// <returns>
        /// The connection data.
        /// </returns>
        protected virtual (ICredentials credentials, string host, int port) GetConnectionData(IEmailMessage emailMessage)
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
            return (new NetworkCredential(settings.UserName, settings.Password), settings.Host, settings.Port ?? EmailSenderSettings.DefaultSmtpPort);
        }
    }
}