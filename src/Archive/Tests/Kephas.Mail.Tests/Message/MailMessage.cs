// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MailMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the mail message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Tests.Message
{
    using System.Collections.Generic;

    using Kephas.Dynamic;

    /// <summary>
    /// A mail message.
    /// </summary>
    public class MailMessage : Expando, IEmailMessage
    {
        /// <summary>
        /// Gets the 'on behalf' sender of the email.
        /// </summary>
        public ICollection<IEmailAddress> From { get; } = new List<IEmailAddress>();

        /// <summary>
        /// Gets the 'on behalf' sender of the email.
        /// </summary>
        IEnumerable<IEmailAddress> IEmailMessage.From => this.From;

        /// <summary>
        /// Gets or sets the sender's address.
        /// </summary>
        public IEmailAddress Sender { get; set; }

        /// <summary>
        /// Gets the addresses of the 'To' recipients.
        /// </summary>
        public ICollection<IEmailAddress> To { get; } = new List<IEmailAddress>();

        /// <summary>
        /// Gets the addresses of the 'To' recipients.
        /// </summary>
        IEnumerable<IEmailAddress> IEmailMessage.To => this.To;

        /// <summary>
        /// Gets the addresses of the 'CC' recipients.
        /// </summary>
        public ICollection<IEmailAddress> Cc { get; } = new List<IEmailAddress>();

        /// <summary>
        /// Gets the addresses of the 'CC' recipients.
        /// </summary>
        IEnumerable<IEmailAddress> IEmailMessage.Cc => this.Cc;

        /// <summary>
        /// Gets the addresses of the 'BCC' recipients.
        /// </summary>
        public ICollection<IEmailAddress> Bcc { get; } = new List<IEmailAddress>();

        /// <summary>
        /// Gets the addresses of the 'BCC' recipients.
        /// </summary>
        IEnumerable<IEmailAddress> IEmailMessage.Bcc => this.Bcc;

        /// <summary>
        /// Gets or sets the subject of the mail.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the HTML body of the email.
        /// </summary>
        public string BodyHtml { get; set; }

        /// <summary>
        /// Gets or sets the text body of the email.
        /// </summary>
        public string BodyText { get; set; }

        /// <summary>
        /// Gets the attachments of the email.
        /// </summary>
        /// <value>
        /// The attachments.
        /// </value>
        public ICollection<IEmailAttachment> Attachments { get; } = new List<IEmailAttachment>();

        /// <summary>
        /// Gets the attachments of the email.
        /// </summary>
        IEnumerable<IEmailAttachment> IEmailMessage.Attachments => this.Attachments;

        public void Dispose()
        {
        }
    }
}