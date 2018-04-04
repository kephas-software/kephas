// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MailMessageBuilder.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the mail message builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Tests.Message
{
    using System.IO;

    using Kephas.Mail.Services;

    using NSubstitute;

    /// <summary>
    /// A mail message builder.
    /// </summary>
    public class MailMessageBuilder : IEmailMessageBuilder
    {
        /// <summary>
        /// Message describing the email.
        /// </summary>
        private readonly MailMessage emailMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="MailMessageBuilder"/> class.
        /// </summary>
        public MailMessageBuilder()
        {
            this.emailMessage = new MailMessage();
        }

        /// <summary>
        /// Gets a message describing the email.
        /// </summary>
        /// <value>
        /// A message describing the email.
        /// </value>
        public IEmailMessage EmailMessage => this.emailMessage;

        /// <summary>
        /// Sets the address from which the mail is sent.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="displayName">Name to display.</param>
        /// <returns>
        /// This <see cref="IEmailMessageBuilder"/>.
        /// </returns>
        public IEmailMessageBuilder From(string address, string displayName = null)
        {
            this.emailMessage.From.Add(this.CreateEmailAddress(address, displayName));

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
            this.emailMessage.Sender = this.CreateEmailAddress(address, displayName);

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
            this.emailMessage.To.Add(this.CreateEmailAddress(address, displayName));

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
            this.emailMessage.Cc.Add(this.CreateEmailAddress(address, displayName));

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
            this.emailMessage.Bcc.Add(this.CreateEmailAddress(address, displayName));

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
            this.emailMessage.BodyHtml = body;
            return this;
        }

        /// <summary>
        /// Sets the HTML body.
        /// </summary>
        /// <param name="body">The HTML body.</param>
        /// <returns>
        /// This <see cref="IEmailMessageBuilder"/>.
        /// </returns>
        public IEmailMessageBuilder BodyText(string body)
        {
            this.emailMessage.BodyText = body;
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
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Creates email address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="displayName">(Optional) Name to display.</param>
        /// <returns>
        /// The new email address.
        /// </returns>
        private IEmailAddress CreateEmailAddress(string address, string displayName = null)
        {
            var emailAddress = Substitute.For<IEmailAddress>();
            emailAddress.Address.Returns(address);
            emailAddress.DisplayName.Returns(displayName);
            return emailAddress;
        }
    }
}