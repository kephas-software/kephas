// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmailMessageBuilderExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the email message builder extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Services
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// Extension methods for <see cref="IEmailMessageBuilder"/>.
    /// </summary>
    public static class EmailMessageBuilderExtensions
    {
        /// <summary>
        /// Sets the recipient addresses.
        /// </summary>
        /// <param name="builder">The builder to act on.</param>
        /// <param name="addresses">The addresses.</param>
        /// <returns>
        /// This <see cref="IEmailMessageBuilder"/>.
        /// </returns>
        public static IEmailMessageBuilder To(
            this IEmailMessageBuilder builder,
            IEnumerable<string> addresses)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            foreach (var address in addresses)
            {
                builder.To(address);
            }

            return builder;
        }        

        /// <summary>
        /// Sets the CC recipient addresses.
        /// </summary>
        /// <param name="builder">The builder to act on.</param>
        /// <param name="addresses">The addresses.</param>
        /// <returns>
        /// This <see cref="IEmailMessageBuilder"/>.
        /// </returns>
        public static IEmailMessageBuilder Cc(
            this IEmailMessageBuilder builder,
            IEnumerable<string> addresses)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            foreach (var address in addresses)
            {
                builder.Cc(address);
            }

            return builder;
        }

        /// <summary>
        /// Sets the BCC recipient addresses.
        /// </summary>
        /// <param name="builder">The builder to act on.</param>
        /// <param name="addresses">The addresses.</param>
        /// <returns>
        /// This <see cref="IEmailMessageBuilder"/>.
        /// </returns>
        public static IEmailMessageBuilder Bcc(
            this IEmailMessageBuilder builder,
            IEnumerable<string> addresses)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            foreach (var address in addresses)
            {
                builder.Bcc(address);
            }

            return builder;
        }        
    }
}