// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmailSenderServiceExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the email sender service extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Services
{
    using System.Threading.Tasks;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Mail.Message;

    /// <summary>
    /// An email sender service extensions.
    /// </summary>
    public static class EmailSenderServiceExtensions
    {
        /// <summary>
        /// An IEmailSenderService extension method that sends an email asynchronously.
        /// </summary>
        /// <param name="senderService">The senderService to act on.</param>
        /// <param name="toAddress">To address.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public static Task SendAsync(this IEmailSenderService senderService, string toAddress, string subject, string body)
        {
            Requires.NotNull(senderService, nameof(senderService));
            Requires.NotNullOrEmpty(toAddress, nameof(toAddress));

            var message = new EmailMessage
                              {
                                  ToRecipients = { new EmailAddress { Address = toAddress } },
                                  Subject = subject,
                                  Body = new EmailBody { Content = body }
                              };
            return senderService.SendAsync(message);
        }

        /// <summary>
        /// An IEmailSenderService extension method that sends an email asynchronously.
        /// </summary>
        /// <param name="senderService">The senderService to act on.</param>
        /// <param name="toAddresses">To addresses.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public static Task SendAsync(this IEmailSenderService senderService, string[] toAddresses, string subject, string body)
        {
            Requires.NotNull(senderService, nameof(senderService));
            Requires.NotNullOrEmpty(toAddresses, nameof(toAddresses));

            var message = new EmailMessage
                              {
                                  Subject = subject,
                                  Body = new EmailBody { Content = body }
                              };
            toAddresses.ForEach(a => message.ToRecipients.Add(new EmailAddress { Address = a }));
            return senderService.SendAsync(message);
        }
    }
}