// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEmailMessage.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IEmailMessage interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail
{
    using System.Collections.Generic;

    using Kephas.Dynamic;

    /// <summary>
    /// Interface for an email message.
    /// </summary>
    public interface IEmailMessage : IExpando
    {
        /// <summary>
        /// Gets or sets the 'on behalf' sender of the email.
        /// </summary>
        IEmailAddress From { get; set; }

        /// <summary>
        /// Gets or sets the sender's address.
        /// </summary>
        IEmailAddress Sender { get; set; }

        /// <summary>
        /// Gets the addresses of the 'To' recipients.
        /// </summary>
        ICollection<IEmailAddress> ToRecipients { get; }

        /// <summary>
        /// Gets the addresses of the 'CC' recipients.
        /// </summary>
        ICollection<IEmailAddress> CcRecipients { get; }

        /// <summary>
        /// Gets the addresses of the 'BCC' recipients.
        /// </summary>
        ICollection<IEmailAddress> BccRecipients { get; }

        /// <summary>
        /// Gets or sets the subject of the mail.
        /// </summary>
        string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body of the email.
        /// </summary>
        IEmailBody Body { get; set; }

        /// <summary>
        /// Gets or sets the attachments of the email.
        /// </summary>
        ICollection<IEmailAttachment> Attachments { get; }
    }
}