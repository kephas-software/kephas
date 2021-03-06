﻿// --------------------------------------------------------------------------------------------------------------------
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
        /// Gets or sets the attachment content.
        /// </summary>
        /// <value>
        /// The attachment content.
        /// </value>
        Stream Content { get; set; }
    }
}