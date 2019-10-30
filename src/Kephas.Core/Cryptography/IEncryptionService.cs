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
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Application service contract for encryption.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IEncryptionService
    {
        /// <summary>
        /// Generates a key.
        /// </summary>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <returns>
        /// An array of byte.
        /// </returns>
        byte[] GenerateKey(Action<IEncryptionContext> optionsConfig = null);

        /// <summary>
        /// Encrypts the input stream and writes the encrypted content into the output stream.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        Task EncryptAsync(
            Stream input,
            Stream output,
            Action<IEncryptionContext> optionsConfig = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Decrypts the input stream and writes the decrypted content into the output stream.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        Task DecryptAsync(
            Stream input,
            Stream output,
            Action<IEncryptionContext> optionsConfig = null,
            CancellationToken cancellationToken = default);
    }
}