// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MailKitEmailMessage.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the mail kit email message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Message
{
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Dynamic;

    using MimeKit;

    /// <summary>
    /// A mail kit email message.
    /// </summary>
    public class MailKitEmailMessage : MimeMessage, IEmailMessage
    {
        /// <summary>
        /// Gets the 'on behalf' sender of the email.
        /// </summary>
        IEnumerable<IEmailAddress> IEmailMessage.From => this.From.Select(a => (IEmailAddress)a);

        /// <summary>
        /// Gets or sets the sender's address.
        /// </summary>
        IEmailAddress IEmailMessage.Sender
        {
            get => (IEmailAddress)this.Sender;
            set => this.Sender = (MailKitEmailAddress)value;
        }

        /// <summary>
        /// Gets the addresses of the 'To' recipients.
        /// </summary>
        IEnumerable<IEmailAddress> IEmailMessage.To => this.To.Select(a => (IEmailAddress)a);

        /// <summary>
        /// Gets the addresses of the 'CC' recipients.
        /// </summary>
        IEnumerable<IEmailAddress> IEmailMessage.Cc => this.Cc.Select(a => (IEmailAddress)a);

        /// <summary>
        /// Gets the addresses of the 'BCC' recipients.
        /// </summary>
        IEnumerable<IEmailAddress> IEmailMessage.Bcc => this.From.Select(a => (IEmailAddress)a);

        /// <summary>
        /// Gets or sets the HTML body of the email.
        /// </summary>
        string IEmailMessage.BodyHtml
        {
            get => this.HtmlBody;
            set => this.Body = new BodyBuilder { HtmlBody = value }.ToMessageBody();
        }

        /// <summary>
        /// Gets or sets the text body of the email.
        /// </summary>
        string IEmailMessage.BodyText
        {
            get => this.TextBody;
            set => this.Body = new BodyBuilder { TextBody = value }.ToMessageBody();
        }

        /// <summary>
        /// Gets the attachments of the email.
        /// </summary>
        IEnumerable<IEmailAttachment> IEmailMessage.Attachments => this.Attachments.Select(a => new MailKitEmailAttachment(a));

        /// <summary>
        /// Convenience method that provides a string Indexer
        /// to the Properties collection AND the strongly typed
        /// properties of the object by name.
        /// // dynamic
        /// exp["Address"] = "112 nowhere lane";
        /// // strong
        /// var name = exp["StronglyTypedProperty"] as string;.
        /// </summary>
        /// <value>
        /// The <see cref="object" /> identified by the key.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns>The requested property value.</returns>
        public object this[string key]
        {
            get => new Expando(this)[key];
            set => new Expando(this)[key] = value;
        }
    }
}