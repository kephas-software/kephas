// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MailKitEmailAddressBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Message
{

    using Kephas.Dynamic;

    using MimeKit;

    /// <summary>
    /// Base class for email address.
    /// </summary>
    /// <typeparam name="T">The adapted type.</typeparam>
    /// <seealso cref="IEmailAddress" />
    /// <seealso cref="IExpandoMixin" />
    /// <seealso cref="IAdapter{T}" />
    public abstract class MailKitEmailAddressBase<T> : Expando, IEmailAddress, IAdapter<T>
        where T : notnull, InternetAddress
    {
        /// <summary>
        /// The adapted address.
        /// </summary>
        protected readonly T address;

        /// <summary>
        /// Initializes a new instance of the <see cref="MailKitEmailAddressBase{T}"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        protected MailKitEmailAddressBase(T address)
            : base(address)
        {
            this.address = address;
        }

        /// <summary>
        /// Gets the address.
        /// </summary>
        public abstract string Address { get; }

        /// <summary>
        /// Gets the display name of the subject associated to the address.
        /// </summary>
        public virtual string DisplayName => this.address.Name;

        /// <summary>
        /// Gets the object the current instance adapts.
        /// </summary>
        /// <value>
        /// The object the current instance adapts.
        /// </value>
        T IAdapter<T>.Of => this.address;
    }
}