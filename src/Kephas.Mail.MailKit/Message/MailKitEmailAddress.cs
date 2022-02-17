// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MailKitEmailAddress.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the mail kit email address class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Message
{
    using System.Collections.Generic;
    using System.Text;

    using Kephas.Dynamic;

    using MimeKit;

    /// <summary>
    /// A mail kit email address.
    /// </summary>
    public class MailKitEmailAddress : MailboxAddress, IEmailAddress, IExpandoMixin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MailKitEmailAddress" /> class.
        /// </summary>
        /// <remarks>
        /// Creates a new <see cref="MailKitEmailAddress" /> with the specified name, address and route. The
        /// specified text encoding is used when encoding the name according to the rules of rfc2047.
        /// </remarks>
        /// <param name="encoding">The character encoding to be used for encoding the name.</param>
        /// <param name="name">The name of the mailbox.</param>
        /// <param name="route">The route of the mailbox.</param>
        /// <param name="address">The address of the mailbox.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <para><paramref name="encoding" /> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para><paramref name="route" /> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para><paramref name="address" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="T:MimeKit.ParseException">
        /// <paramref name="address" /> is malformed.
        /// </exception>
        public MailKitEmailAddress(Encoding encoding, string name, IEnumerable<string> route, string address)
            : base(encoding, name, route, address)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MailKitEmailAddress" /> class.
        /// </summary>
        /// <remarks>
        /// Creates a new <see cref="MailKitEmailAddress" /> with the specified name, address and route.
        /// </remarks>
        /// <param name="name">The name of the mailbox.</param>
        /// <param name="route">The route of the mailbox.</param>
        /// <param name="address">The address of the mailbox.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <para><paramref name="route" /> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para><paramref name="address" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="T:MimeKit.ParseException">
        /// <paramref name="address" /> is malformed.
        /// </exception>
        public MailKitEmailAddress(string name, IEnumerable<string> route, string address)
            : base(name, route, address)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MailKitEmailAddress" /> class.
        /// </summary>
        /// <remarks>
        /// Creates a new <see cref="MailKitEmailAddress" /> with the specified name and address. The
        /// specified text encoding is used when encoding the name according to the rules of rfc2047.
        /// </remarks>
        /// <param name="encoding">The character encoding to be used for encoding the name.</param>
        /// <param name="name">The name of the mailbox.</param>
        /// <param name="address">The address of the mailbox.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <para><paramref name="encoding" /> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para><paramref name="address" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="T:MimeKit.ParseException">
        /// <paramref name="address" /> is malformed.
        /// </exception>
        public MailKitEmailAddress(Encoding encoding, string name, string address)
            : base(encoding, name, address)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MailKitEmailAddress" /> class.
        /// </summary>
        /// <remarks>
        /// Creates a new <see cref="MailKitEmailAddress" /> with the specified name and address.
        /// </remarks>
        /// <param name="name">The name of the mailbox.</param>
        /// <param name="address">The address of the mailbox.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="address" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="T:MimeKit.ParseException">
        /// <paramref name="address" /> is malformed.
        /// </exception>
        public MailKitEmailAddress(string name, string address)
            : base(name, address)
        {
        }

        /// <summary>
        /// Gets the inner dictionary.
        /// </summary>
        IDictionary<string, object?> IExpandoMixin.InnerDictionary { get; } = new Dictionary<string, object?>();

        /// <summary>
        /// Gets the display name of the subject associated to the address.
        /// </summary>
        string IEmailAddress.DisplayName => this.Name;
    }
}