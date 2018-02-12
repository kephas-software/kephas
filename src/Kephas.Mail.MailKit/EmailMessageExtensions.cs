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
    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;

    using MimeKit;

    /// <summary>
    /// Extensions for <see cref="IEmailMessage"/>.
    /// </summary>
    public static class EmailMessageExtensions
    {
        /// <summary>
        /// Converts the <see cref="IEmailMessage"/> to a <see cref="MimeMessage"/>.
        /// </summary>
        /// <param name="emailMessage">The email message to act on.</param>
        /// <returns>
        /// <paramref name="emailMessage"/> as a <see cref="MimeMessage"/>.
        /// </returns>
        public static MimeMessage ToMailMessage(this IEmailMessage emailMessage)
        {
            Requires.NotNull(emailMessage, nameof(emailMessage));

            var message = new MimeMessage
            {
                Sender = emailMessage.Sender?.ToMailAddress(),
                Subject = emailMessage.Subject,
            };

            emailMessage.ToRecipients.ForEach(r => message.To.Add(r.ToMailAddress()));
            emailMessage.CcRecipients.ForEach(r => message.Cc.Add(r.ToMailAddress()));
            emailMessage.BccRecipients.ForEach(r => message.Bcc.Add(r.ToMailAddress()));

            var bodyBuilder = emailMessage.Body.BodyType == EmailBodyType.Text
                                  ? new BodyBuilder { TextBody = emailMessage.Body.Content }
                                  : new BodyBuilder { HtmlBody = emailMessage.Body.Content };

            emailMessage.Attachments.ForEach(a => bodyBuilder.Attachments.Add(a.Name, a.Content));

            message.Body = bodyBuilder.ToMessageBody();

            return message;
        }

        /// <summary>
        /// Converts the <see cref="IEmailAddress"/> to a <see cref="MailboxAddress"/>.
        /// </summary>
        /// <param name="emailAddress">The email address to act on.</param>
        /// <returns>
        /// <paramref name="emailAddress"/> as a <see cref="MailboxAddress"/>.
        /// </returns>
        public static MailboxAddress ToMailAddress(this IEmailAddress emailAddress)
        {
            Requires.NotNull(emailAddress, nameof(emailAddress));

            return string.IsNullOrEmpty(emailAddress.DisplayName)
                       ? new MailboxAddress(emailAddress.Address)
                       : new MailboxAddress(emailAddress.Address, emailAddress.DisplayName);
        }
    }
}