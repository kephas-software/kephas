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
    public class MailKitEmailAddress : MailboxAddress, IEmailAddress
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
        /// Creates a new <see cref="MailKitEmailAddress" /> with the specified address and route.
        /// </remarks>
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
        public MailKitEmailAddress(IEnumerable<string> route, string address)
            : base(route, address)
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
        /// Initializes a new instance of the <see cref="MailKitEmailAddress" /> class.
        /// </summary>
        /// <remarks>
        /// <para>Creates a new <see cref="MailKitEmailAddress" /> with the specified address.</para>
        /// <note type="note">
        /// <para>The <paramref name="address" /> must be in the form <c>user@example.com</c>.</para>
        /// <para>This method cannot be used to parse a free-form email address that includes
        /// the name or encloses the address in angle brackets.</para>
        /// <para>To parse a free-form email address, use <see cref="M:MimeKit.MailboxAddress.Parse(System.String)" /> instead.</para>
        /// </note>
        /// </remarks>
        /// <param name="address">The address of the mailbox.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="address" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="T:MimeKit.ParseException">
        /// <paramref name="address" /> is malformed.
        /// </exception>
        public MailKitEmailAddress(string address)
            : base(address)
        {
        }

        /// <summary>
        /// Gets the display name of the subject associated to the address.
        /// </summary>
        string IEmailAddress.DisplayName => this.Name;

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
            get => new Expando(this)[key];
            set => new Expando(this)[key] = value;
        }
    }
}