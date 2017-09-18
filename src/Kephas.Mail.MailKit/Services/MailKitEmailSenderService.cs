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

    using Kephas.Threading.Tasks;

    using MailKit.Net.Smtp;
    using MailKit.Security;

    /// <summary>
    /// A MailKit email sender service.
    /// </summary>
    public class MailKitEmailSenderService : IEmailSenderService
    {
        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="emailMessage">The email message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public async Task SendAsync(IEmailMessage emailMessage, CancellationToken cancellationToken = default)
        {
            var smtpClient = new SmtpClient();

            (var credentials, var host, var port) = this.GetConnectionData(emailMessage);
            await smtpClient.AuthenticateAsync(credentials, cancellationToken).PreserveThreadContext();
            await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTlsWhenAvailable, cancellationToken).PreserveThreadContext();

            try
            {
                await smtpClient.SendAsync(emailMessage.ToMailMessage(), cancellationToken).PreserveThreadContext();
            }
            finally
            {
                await smtpClient.DisconnectAsync(quit: true, cancellationToken: cancellationToken).PreserveThreadContext();
            }
        }

        /// <summary>
        /// Gets the connection data.
        /// </summary>
        /// <param name="emailMessage">The email message.</param>
        /// <returns>
        /// The connection data.
        /// </returns>
        protected virtual (ICredentials credentials, string host, int port) GetConnectionData(IEmailMessage emailMessage)
        {
            // TODO get the connection data from the configuration service (IConfiguration<Type>).
            return (null, null, 0);
        }
    }
}