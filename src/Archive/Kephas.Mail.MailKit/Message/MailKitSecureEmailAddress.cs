// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MailKitSecureEmailAddress.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Message;

using MimeKit.Cryptography;

/// <summary>
/// A secure mailbox address.
/// </summary>
public class MailKitSecureMailboxAddress : MailKitEmailAddressBase<SecureMailboxAddress>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MailKitSecureMailboxAddress"/> class.
    /// </summary>
    /// <param name="address">The address.</param>
    internal MailKitSecureMailboxAddress(SecureMailboxAddress address)
        : base(address)
    {
    }

    /// <summary>
    /// Gets the address.
    /// </summary>
    public override string Address => this.address.Address;
}
