// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEncryptionContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IEncryptionContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Cryptography
{
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
        byte[] Key { get; set; }

        /// <summary>
        /// Gets or sets the size of the key.
        /// </summary>
        /// <value>
        /// The size of the key.
        /// </value>
        int? KeySize { get; set; }
    }
}