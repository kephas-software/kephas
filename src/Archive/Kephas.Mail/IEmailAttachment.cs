// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEmailAttachment.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IEmailAttachment interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail
{
    using System.IO;

    using Kephas.Dynamic;

    /// <summary>
    /// An email attachment.
    /// </summary>
    public interface IEmailAttachment : IDynamic
    {
        /// <summary>
        /// Gets or sets the attachment name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; set; }

        /// <summary>
        /// Writes the content of the attachment to the specified output stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public void WriteContentTo(Stream stream, CancellationToken cancellationToken = default);

        /// <summary>
        /// Writes the content of the attachment to the specified output stream asynchronously.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        public Task WriteContentToAsync(Stream stream, CancellationToken cancellationToken = default);
    }
}