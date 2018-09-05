// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MailKitEmailMessageBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the mail kit email message builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Services
{
    using System.IO;

    using Kephas.Mail.Message;

    using MimeKit;

    /// <summary>
    /// A mail kit email message builder.
    /// </summary>
    public class MailKitEmailMessageBuilder : IEmailMessageBuilder
    {
        /// <summary>
        /// Message describing the email.
        /// </summary>
        private readonly MailKitEmailMessage emailMessage;

        /// <summary>
        /// The body builder.
        /// </summary>
        private readonly BodyBuilder bodyBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="MailKitEmailMessageBuilder"/> class.
        /// </summary>
        public MailKitEmailMessageBuilder()
        {
            this.emailMessage = new MailKitEmailMessage();
            this.bodyBuilder = new BodyBuilder();
        }

        /// <summary>
        /// Gets the email message.
        /// </summary>
        /// <value>
        /// The email message.
        /// </value>
        public IEmailMessage EmailMessage => this.emailMessage;

        /// <summary>
        /// Sets the 'on behalf' recipient address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="displayName">Name to display.</param>
        /// <returns>
        /// This <see cref="IEmailMessageBuilder"/>.
        /// </returns>
        public IEmailMessageBuilder From(string address, string displayName = null)
        {
            if (string.IsNullOrEmpty(displayName))
            {
                this.emailMessage.From.Add(new MailKitEmailAddress(address));
            }
            else
            {
                this.emailMessage.From.Add(new MailKitEmailAddress(address, displayName));
            }

            return this;
        }

        /// <summary>
        /// Sets the sender address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="displayName">Name to display.</param>
        /// <returns>
        /// This <see cref="IEmailMessageBuilder"/>.
        /// </returns>
        public IEmailMessageBuilder Sender(string address, string displayName = null)
        {
            if (string.IsNullOrEmpty(displayName))
            {
                this.emailMessage.Sender = new MailKitEmailAddress(address);
            }
            else
            {
                this.emailMessage.Sender = new MailKitEmailAddress(address, displayName);
            }

            return this;
        }

        /// <summary>
        /// Sets the recipient address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="displayName">Name to display.</param>
        /// <returns>
        /// This <see cref="IEmailMessageBuilder"/>.
        /// </returns>
        public IEmailMessageBuilder To(string address, string displayName = null)
        {
            if (string.IsNullOrEmpty(displayName))
            {
                this.emailMessage.To.Add(new MailKitEmailAddress(address));
            }
            else
            {
                this.emailMessage.To.Add(new MailKitEmailAddress(address, displayName));
            }

            return this;
        }

        /// <summary>
        /// Sets the carbon copy recipient address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="displayName">Name to display.</param>
        /// <returns>
        /// This <see cref="IEmailMessageBuilder"/>.
        /// </returns>
        public IEmailMessageBuilder Cc(string address, string displayName = null)
        {
            if (string.IsNullOrEmpty(displayName))
            {
                this.emailMessage.Cc.Add(new MailKitEmailAddress(address));
            }
            else
            {
                this.emailMessage.Cc.Add(new MailKitEmailAddress(address, displayName));
            }

            return this;
        }

        /// <summary>
        /// Sets the blind carbon copy recipient address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="displayName">Name to display.</param>
        /// <returns>
        /// This <see cref="IEmailMessageBuilder"/>.
        /// </returns>
        public IEmailMessageBuilder Bcc(string address, string displayName = null)
        {
            if (string.IsNullOrEmpty(displayName))
            {
                this.emailMessage.Bcc.Add(new MailKitEmailAddress(address));
            }
            else
            {
                this.emailMessage.Bcc.Add(new MailKitEmailAddress(address, displayName));
            }

            return this;
        }

        /// <summary>
        /// Sets the subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <returns>
        /// This <see cref="IEmailMessageBuilder"/>.
        /// </returns>
        public IEmailMessageBuilder Subject(string subject)
        {
            this.emailMessage.Subject = subject;

            return this;
        }

        /// <summary>
        /// Sets the HTML body.
        /// </summary>
        /// <param name="body">The HTML body.</param>
        /// <returns>
        /// This <see cref="IEmailMessageBuilder"/>.
        /// </returns>
        public IEmailMessageBuilder BodyHtml(string body)
        {
            this.bodyBuilder.HtmlBody = body;
            this.emailMessage.Body = this.bodyBuilder.ToMessageBody();

            return this;
        }

        /// <summary>
        /// Sets the text body.
        /// </summary>
        /// <param name="body">The text body.</param>
        /// <returns>
        /// This <see cref="IEmailMessageBuilder"/>.
        /// </returns>
        public IEmailMessageBuilder BodyText(string body)
        {
            this.bodyBuilder.TextBody = body;
            this.emailMessage.Body = this.bodyBuilder.ToMessageBody();

            return this;
        }

        /// <summary>
        /// Adds an attachment to the message.
        /// </summary>
        /// <param name="content">The attachment content.</param>
        /// <param name="name">The attachment name.</param>
        /// <returns>
        /// This <see cref="IEmailMessageBuilder"/>.
        /// </returns>
        public IEmailMessageBuilder AddAttachment(Stream content, string name)
        {
            this.bodyBuilder.Attachments.Add(name, content);
            this.emailMessage.Body = this.bodyBuilder.ToMessageBody();

            return this;
        }
    }
}