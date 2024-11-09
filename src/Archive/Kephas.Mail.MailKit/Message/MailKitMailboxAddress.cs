// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MailKitMailboxAddress.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Message;

using MimeKit;

/// <summary>
/// A mailbox address.
/// </summary>
public class MailKitMailboxAddress : MailKitEmailAddressBase<MailboxAddress>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MailKitMailboxAddress"/> class.
    /// </summary>
    /// <param name="address">The address.</param>
    internal MailKitMailboxAddress(MailboxAddress address)
        : base(address)
    {
    }

    /// <summary>
    /// Gets the address.
    /// </summary>
    public override string Address => this.address.Address;
}
