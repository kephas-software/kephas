// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EncryptionContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the encryption context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Cryptography
{
    using Kephas.Composition;
    using Kephas.Services;

    /// <summary>
    /// An encryption context.
    /// </summary>
    public class EncryptionContext : Context, IEncryptionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptionContext"/> class.
        /// </summary>
        /// <param name="injector">The composition context.</param>
        public EncryptionContext(IInjector injector)
            : base(injector)
        {
        }

        /// <summary>
        /// Gets or sets the key for encryption/decryption.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public byte[]? Key { get; set; }

        /// <summary>
        /// Gets or sets the size of the key.
        /// </summary>
        /// <value>
        /// The size of the key.
        /// </value>
        public int? KeySize { get; set; }
    }
}