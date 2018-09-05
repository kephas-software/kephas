// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEmailMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    public interface IEmailMessage : IIndexable
    {
        /// <summary>
        /// Gets the 'on behalf' sender of the email.
        /// </summary>
        IEnumerable<IEmailAddress> From { get; }

        /// <summary>
        /// Gets or sets the sender's address.
        /// </summary>
        IEmailAddress Sender { get; set; }

        /// <summary>
        /// Gets the addresses of the 'To' recipients.
        /// </summary>
        IEnumerable<IEmailAddress> To { get; }

        /// <summary>
        /// Gets the addresses of the 'CC' recipients.
        /// </summary>
        IEnumerable<IEmailAddress> Cc { get; }

        /// <summary>
        /// Gets the addresses of the 'BCC' recipients.
        /// </summary>
        IEnumerable<IEmailAddress> Bcc { get; }

        /// <summary>
        /// Gets or sets the subject of the mail.
        /// </summary>
        string Subject { get; set; }

        /// <summary>
        /// Gets or sets the HTML body of the email.
        /// </summary>
        string BodyHtml { get; set; }

        /// <summary>
        /// Gets or sets the text body of the email.
        /// </summary>
        string BodyText { get; set; }

        /// <summary>
        /// Gets the attachments of the email.
        /// </summary>
        IEnumerable<IEmailAttachment> Attachments { get; }
    }
}