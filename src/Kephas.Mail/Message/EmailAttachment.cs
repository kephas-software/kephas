// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmailAttachment.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the email attachment class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Message
{
    using System.IO;

    using Kephas.Dynamic;

    /// <summary>
    /// An email attachment.
    /// </summary>
    public class EmailAttachment : Expando, IEmailAttachment
    {
        /// <summary>
        /// Gets or sets the attachment name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the attachment content.
        /// </summary>
        /// <value>
        /// The attachment content.
        /// </value>
        public Stream Content { get; set; }
    }
}