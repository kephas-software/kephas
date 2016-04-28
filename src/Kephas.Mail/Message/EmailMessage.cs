// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmailMessage.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the email message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Message
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Kephas.Dynamic;

    /// <summary>
    /// An email message.
    /// </summary>
    public class EmailMessage : Expando, IEmailMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailMessage"/> class.
        /// </summary>
        public EmailMessage()
        {
            this.ToRecipients = new Collection<IEmailAddress>();
            this.CcRecipients = new Collection<IEmailAddress>();
            this.BccRecipients = new Collection<IEmailAddress>();
            this.Attachments = new Collection<IEmailAttachment>();
        }

        /// <summary>
        /// Gets or sets the 'on behalf' sender of the email.
        /// </summary>
        public IEmailAddress From { get; set; }

        /// <summary>
        /// Gets or sets the sender's address.
        /// </summary>
        public IEmailAddress Sender { get; set; }

        /// <summary>
        /// Gets the addresses of the 'To' recipients.
        /// </summary>
        public ICollection<IEmailAddress> ToRecipients { get; }

        /// <summary>
        /// Gets the addresses of the 'CC' recipients.
        /// </summary>
        public ICollection<IEmailAddress> CcRecipients { get; }

        /// <summary>
        /// Gets the addresses of the 'BCC' recipients.
        /// </summary>
        public ICollection<IEmailAddress> BccRecipients { get; }

        /// <summary>
        /// Gets or sets the subject of the mail.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body of the email.
        /// </summary>
        public IEmailBody Body { get; set; }

        /// <summary>
        /// Gets the attachments of the email.
        /// </summary>
        public ICollection<IEmailAttachment> Attachments { get; }
    }
}