// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MailKitEmailAttachment.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the mail kit email attachment class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Message
{
    using System;
    using System.IO;

    using MimeKit;

    /// <summary>
    /// A mail kit email attachment.
    /// </summary>
    public class MailKitEmailAttachment : IEmailAttachment
    {
        /// <summary>
        /// The attachment.
        /// </summary>
        private readonly MimeEntity attachment;

        /// <summary>
        /// Initializes a new instance of the <see cref="MailKitEmailAttachment"/> class.
        /// </summary>
        /// <param name="attachment">The attachment.</param>
        internal MailKitEmailAttachment(MimeEntity attachment)
        {
            this.attachment = attachment;
        }

        /// <summary>
        /// Gets or sets the attachment name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get => this.attachment.ContentDisposition.FileName;
            set => this.attachment.ContentDisposition.FileName = value;
        }

        /// <summary>
        /// Gets or sets the attachment content.
        /// </summary>
        /// <value>
        /// The attachment content.
        /// </value>
        public Stream Content
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

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
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }
    }
}