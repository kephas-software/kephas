// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEncryptionContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    }
}