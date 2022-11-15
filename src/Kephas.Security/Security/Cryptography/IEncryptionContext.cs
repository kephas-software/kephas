// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEncryptionContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IEncryptionContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Cryptography
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// Interface for encryption context.
    /// </summary>
    public interface IEncryptionContext : IContext
    {
        /// <summary>
        /// Gets or sets the key for encryption/decryption.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        byte[]? Key { get; set; }

        /// <summary>
        /// Gets or sets the size of the key.
        /// </summary>
        /// <value>
        /// The size of the key.
        /// </value>
        int? KeySize { get; set; }
    }

    /// <summary>
    /// Extension methods for <see cref="IEncryptionContext"/>.
    /// </summary>
    public static class EncryptionContextExtensions
    {
        /// <summary>
        /// Sets the key.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The encryption context.</param>
        /// <param name="key">The key.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        public static TContext Key<TContext>(this TContext context, byte[]? key)
            where TContext : class, IEncryptionContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            context.Key = key;

            return context;
        }

        /// <summary>
        /// Sets the key size.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The encryption context.</param>
        /// <param name="keySize">Size of the key.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        public static TContext KeySize<TContext>(this TContext context, int? keySize)
            where TContext : class, IEncryptionContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            context.KeySize = keySize;

            return context;
        }
    }
}