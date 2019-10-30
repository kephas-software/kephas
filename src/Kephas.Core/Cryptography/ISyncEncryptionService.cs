// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISyncEncryptionService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ISyncEncryptionService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Cryptography
{
    using System;
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
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        void Encrypt(
            Stream input,
            Stream output,
            Action<IEncryptionContext> optionsConfig = null);

        /// <summary>
        /// Decrypts the input stream and writes the decrypted content into the output stream.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        void Decrypt(
            Stream input,
            Stream output,
            Action<IEncryptionContext> optionsConfig = null);
    }
}