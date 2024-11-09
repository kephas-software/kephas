// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MailKitHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Mail.Message;
using MimeKit.Cryptography;
using MimeKit;

namespace Kephas.Mail;

/// <summary>
/// Helper class for MailKit entities.
/// </summary>
public static class MailKitHelper
{

    /// <summary>
    /// Converts the provided address to an <see cref="IEmailAddress"/>.
    /// </summary>
    /// <param name="address">The internet address.</param>
    /// <returns>The converted <see cref="IEmailAddress"/>.</returns>
    public static IEmailAddress ToEmailAddress(this InternetAddress address)
    {
        return address switch
        {
            SecureMailboxAddress secureMailboAddress => new MailKitSecureMailboxAddress(secureMailboAddress),
            MailboxAddress mailboxAddress => new MailKitMailboxAddress(mailboxAddress),
            GroupAddress groupAddress => new MailKitGroupAddress(groupAddress),
            _ => throw new NotSupportedException($"{address.GetType()} is not supported."),
        };
    }

    /// <summary>
    /// Converts the provided <see cref="MimeEntity"/> to an <see cref="IEmailAttachment"/>.
    /// </summary>
    /// <param name="mimeEntity">The MIME entity.</param>
    /// <returns>The converted <see cref="IEmailAttachment"/>.</returns>
    public static IEmailAttachment ToEmailAttachment(this MimeEntity mimeEntity)
    {
        return new MailKitMimeEntity(mimeEntity);
    }
}
