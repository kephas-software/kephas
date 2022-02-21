﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MailKitMimeEntity.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Message
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using MimeKit;

    /// <summary>
    /// A mail kit email attachment.
    /// </summary>
    public class MailKitMimeEntity : Expando, IEmailAttachment, IAdapter<MimeEntity>
    {
        private readonly MimeEntity attachment;

        /// <summary>
        /// Initializes a new instance of the <see cref="MailKitMimeEntity"/> class.
        /// </summary>
        /// <param name="attachment">The attachment.</param>
        internal MailKitMimeEntity(MimeEntity attachment)
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
        /// Gets the object the current instance adapts.
        /// </summary>
        /// <value>
        /// The object the current instance adapts.
        /// </value>
        MimeEntity IAdapter<MimeEntity>.Of => this.attachment;

        /// <summary>
        /// Writes the content of the attachment to the specified output stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public void WriteContentTo(Stream stream, CancellationToken cancellationToken = default)
        {
            this.attachment.WriteTo(stream, true, cancellationToken);
        }

        /// <summary>
        /// Writes the content of the attachment to the specified output stream asynchronously.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public Task WriteContentToAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            return this.attachment.WriteToAsync(stream, true, cancellationToken);
        }
    }
}