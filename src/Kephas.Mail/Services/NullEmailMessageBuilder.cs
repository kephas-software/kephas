// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullEmailMessageBuilder.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the null email message builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Services
{
    using System.IO;

    /// <summary>
    /// A null email message builder.
    /// </summary>
    public class NullEmailMessageBuilder : IEmailMessageBuilder
    {
        /// <summary>
        /// Gets a message describing the email.
        /// </summary>
        /// <value>
        /// A message describing the email.
        /// </value>
        public IEmailMessage EmailMessage => null;

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
            return this;
        }

        /// <summary>
        /// Adds an attachment to the message.
        /// </summary>
        /// <param name="content">The attachment content.</param>
        /// <param name="name">The file name.</param>
        /// <returns>
        /// This <see cref="IEmailMessageBuilder"/>.
        /// </returns>
        public IEmailMessageBuilder AddAttachment(Stream content, string name)
        {
            return this;
        }
    }
}