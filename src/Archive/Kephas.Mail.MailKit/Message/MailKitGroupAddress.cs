// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MailKitGroupAddress.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Message;

using MimeKit;

/// <summary>
/// Gets an adapter for the <see cref="GroupAddress"/>.
/// </summary>
public class MailKitGroupAddress : MailKitEmailAddressBase<GroupAddress>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MailKitGroupAddress"/> class.
    /// </summary>
    /// <param name="address">The address.</param>
    internal MailKitGroupAddress(GroupAddress address)
        : base(address)
    {
    }

    /// <summary>
    /// Gets the addresses of the group members concatenated in a string separated by semicolon (;).
    /// </summary>
    public override string Address => string.Join(";", this.address.Members.Select(m => m.ToEmailAddress().Address));
}
