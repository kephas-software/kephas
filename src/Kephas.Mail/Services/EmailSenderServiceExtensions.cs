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

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// An email sender service extensions.
    /// </summary>
    public static class EmailSenderServiceExtensions
    {
        /// <summary>
        /// An IEmailSenderService extension method that sends an email asynchronously.
        /// </summary>
        /// <param name="senderService">The senderService to act on.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public static Task SendAsync(this IEmailSenderService senderService, string subject, string body)
        {
            Requires.NotNull(senderService, nameof(senderService));

            var messageBuilder = senderService.CreateEmailMessageBuilder();
            var message = messageBuilder.Subject(subject).BodyHtml(body).EmailMessage;
            return senderService.SendAsync(message);
        }

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

            var messageBuilder = senderService.CreateEmailMessageBuilder();
            var message = messageBuilder.To(toAddress).Subject(subject).BodyHtml(body).EmailMessage;
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

            var messageBuilder = senderService.CreateEmailMessageBuilder();
            var message = messageBuilder.To(toAddresses).Subject(subject).BodyHtml(body).EmailMessage;
            return senderService.SendAsync(message);
        }
    }
}