// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EncryptionContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the encryption context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Cryptography
{
    using Kephas.Services;

    /// <summary>
    /// An encryption context.
    /// </summary>
    public class EncryptionContext : Context, IEncryptionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptionContext"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services (optional).</param>
        /// <param name="isThreadSafe">True if this object is thread safe (optional).</param>
        public EncryptionContext(IAmbientServices ambientServices = null, bool isThreadSafe = false)
            : base(ambientServices, isThreadSafe)
        {
        }

        /// <summary>
        /// Gets or sets the key for encryption/decryption.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public byte[] Key { get; set; }
    }
}