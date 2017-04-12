﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SmtpEmailSenderService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the SMTP email sender service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Net46.Services
{
    using System.Net.Mail;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Mail.Services;

    /// <summary>
    /// A SMTP email sender service.
    /// </summary>
    public class SmtpEmailSenderService : IEmailSenderService
    {
        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="emailMessage">The email message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public Task SendAsync(IEmailMessage emailMessage, CancellationToken cancellationToken = default(CancellationToken))
        {
            var smtpClient = this.CreateSmtpClient();

            return smtpClient.SendMailAsync(emailMessage.ToMailMessage());
        }

        /// <summary>
        /// Creates the SMTP client.
        /// </summary>
        /// <returns>
        /// The new SMTP client.
        /// </returns>
        protected virtual SmtpClient CreateSmtpClient()
        {
            return new SmtpClient();
        }
    }
}