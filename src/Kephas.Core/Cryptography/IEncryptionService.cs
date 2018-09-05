// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEncryptionService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IEncryptionService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Cryptography
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Application service contract for encryption.
    /// </summary>
    [SharedAppServiceContract]
    public interface IEncryptionService
    {
        /// <summary>
        /// Generates a key.
        /// </summary>
        /// <param name="encryptionContext">Optional. Context for the encryption.</param>
        /// <returns>
        /// An array of byte.
        /// </returns>
        byte[] GenerateKey(IEncryptionContext encryptionContext = null);

        /// <summary>
        /// Encrypts the input stream and writes the encrypted content into the output stream.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="context">The encryption context (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        Task EncryptAsync(
            Stream input,
            Stream output,
            IEncryptionContext context = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Decrypts the input stream and writes the decrypted content into the output stream.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="context">The encryption context (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        Task DecryptAsync(
            Stream input,
            Stream output,
            IEncryptionContext context = null,
            CancellationToken cancellationToken = default);
    }
}