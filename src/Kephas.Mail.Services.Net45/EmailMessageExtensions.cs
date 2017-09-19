// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmailMessageExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the email message extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail
{
    using System.Net.Mail;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Extensions for <see cref="IEmailMessage"/>.
    /// </summary>
    public static class EmailMessageExtensions
    {
        /// <summary>
        /// Converts the <see cref="IEmailMessage"/> to a <see cref="MailMessage"/>.
        /// </summary>
        /// <param name="emailMessage">The email message to act on.</param>
        /// <returns>
        /// <paramref name="emailMessage"/> as a <see cref="MailMessage"/>.
        /// </returns>
        public static MailMessage ToMailMessage(this IEmailMessage emailMessage)
        {
            Requires.NotNull(emailMessage, nameof(emailMessage));

            var message = new MailMessage
                              {
                                  From = emailMessage.From.ToMailAddress(),
                                  Sender = emailMessage.Sender.ToMailAddress(),
                                  Subject = emailMessage.Subject,
                                  IsBodyHtml = emailMessage.Body.BodyType == EmailBodyType.Html,
                                  Body = emailMessage.Body.Content
                              };

            emailMessage.ToRecipients.ForEach(r => message.To.Add(r.ToMailAddress()));
            emailMessage.CcRecipients.ForEach(r => message.CC.Add(r.ToMailAddress()));
            emailMessage.BccRecipients.ForEach(r => message.Bcc.Add(r.ToMailAddress()));
            emailMessage.Attachments.ForEach(a => message.Attachments.Add(a.ToAttachment()));

            return message;
        }

        /// <summary>
        /// Converts the <see cref="IEmailAddress"/> to a <see cref="MailAddress"/>.
        /// </summary>
        /// <param name="emailAddress">The email address to act on.</param>
        /// <returns>
        /// <paramref name="emailAddress"/> as a <see cref="MailAddress"/>.
        /// </returns>
        public static MailAddress ToMailAddress(this IEmailAddress emailAddress)
        {
            Requires.NotNull(emailAddress, nameof(emailAddress));

            return string.IsNullOrEmpty(emailAddress.DisplayName)
                       ? new MailAddress(emailAddress.Address)
                       : new MailAddress(emailAddress.Address, emailAddress.DisplayName);
        }

        /// <summary>
        /// Converts the <see cref="IEmailAttachment"/> to an <see cref="Attachment"/>.
        /// </summary>
        /// <param name="emailAttachment">The email attachment to act on.</param>
        /// <returns>
        /// <paramref name="emailAttachment"/> as an <see cref="Attachment"/>.
        /// </returns>
        public static Attachment ToAttachment(this IEmailAttachment emailAttachment)
        {
            Requires.NotNull(emailAttachment, nameof(emailAttachment));

            return new Attachment(emailAttachment.Content, emailAttachment.Name);
        }
    }
}
