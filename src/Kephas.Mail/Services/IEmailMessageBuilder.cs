// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEmailMessageBuilder.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IEmailMessageBuilder interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Kephas.Mail.Services
{
    using System.IO;

    /// <summary>
    /// Interface for email message builder.
    /// </summary>
    public interface IEmailMessageBuilder
    {
        /// <summary>
        /// Gets a message describing the email.
        /// </summary>
        /// <value>
        /// A message describing the email.
        /// </value>
        IEmailMessage EmailMessage { get; }

        /// <summary>
        /// Sets the address from which the mail is sent.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="displayName">Name to display.</param>
        /// <returns>
        /// This <see cref="IEmailMessageBuilder"/>.
        /// </returns>
        IEmailMessageBuilder From(string address, string displayName = null);

        /// <summary>
        /// Sets the sender address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="displayName">Name to display.</param>
        /// <returns>
        /// This <see cref="IEmailMessageBuilder"/>.
        /// </returns>
        IEmailMessageBuilder Sender(string address, string displayName = null);

        /// <summary>
        /// Sets the recipient address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="displayName">Name to display.</param>
        /// <returns>
        /// This <see cref="IEmailMessageBuilder"/>.
        /// </returns>
        IEmailMessageBuilder To(string address, string displayName = null);

        /// <summary>
        /// Sets the carbon copy recipient address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="displayName">Name to display.</param>
        /// <returns>
        /// This <see cref="IEmailMessageBuilder"/>.
        /// </returns>
        IEmailMessageBuilder Cc(string address, string displayName = null);

        /// <summary>
        /// Sets the blind carbon copy recipient address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="displayName">Name to display.</param>
        /// <returns>
        /// This <see cref="IEmailMessageBuilder"/>.
        /// </returns>
        IEmailMessageBuilder Bcc(string address, string displayName = null);

        /// <summary>
        /// Sets the subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <returns>
        /// This <see cref="IEmailMessageBuilder"/>.
        /// </returns>
        IEmailMessageBuilder Subject(string subject);

        /// <summary>
        /// Sets the HTML body.
        /// </summary>
        /// <param name="body">The HTML body.</param>
        /// <returns>
        /// This <see cref="IEmailMessageBuilder"/>.
        /// </returns>
        IEmailMessageBuilder BodyHtml(string body);

        /// <summary>
        /// Sets the HTML body.
        /// </summary>
        /// <param name="body">The HTML body.</param>
        /// <returns>
        /// This <see cref="IEmailMessageBuilder"/>.
        /// </returns>
        IEmailMessageBuilder BodyText(string body);

        /// <summary>
        /// Adds an attachment to the message.
        /// </summary>
        /// <param name="content">The attachment content.</param>
        /// <param name="name">The attachment name.</param>
        /// <returns>
        /// This <see cref="IEmailMessageBuilder"/>.
        /// </returns>
        IEmailMessageBuilder AddAttachment(Stream content, string name);
    }
}