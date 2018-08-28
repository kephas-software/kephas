// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISyncEncryptionService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ISyncEncryptionService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Cryptography
{
    using System.IO;

    /// <summary>
    /// Application service contract for synchronous encryption.
    /// </summary>
    public interface ISyncEncryptionService
    {
        /// <summary>
        /// Encrypts the input stream and writes the encrypted content into the output stream.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="context">The encryption context (optional).</param>
        void Encrypt(
            Stream input,
            Stream output,
            IEncryptionContext context = null);

        /// <summary>
        /// Decrypts the input stream and writes the decrypted content into the output stream.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="context">The encryption context (optional).</param>
        void Decrypt(
            Stream input,
            Stream output,
            IEncryptionContext context = null);
    }
}