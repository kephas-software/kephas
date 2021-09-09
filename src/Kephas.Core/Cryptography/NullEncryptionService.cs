// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullEncryptionService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the null encryption service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Cryptography
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A null encryption service.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullEncryptionService : IEncryptionService
    {
        /// <summary>
        /// Generates a key.
        /// </summary>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <returns>
        /// An array of bytes.
        /// </returns>
        public byte[] GenerateKey(Action<IEncryptionContext>? optionsConfig = null)
        {
            return null;
        }

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
        public Task EncryptAsync(Stream input, Stream output, Action<IEncryptionContext>? optionsConfig = null, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(input, nameof(input));
            Requires.NotNull(output, nameof(output));

            return ReverseStreamAsync(input, output, cancellationToken);
        }

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
        public Task DecryptAsync(Stream input, Stream output, Action<IEncryptionContext>? optionsConfig = null, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(input, nameof(input));
            Requires.NotNull(output, nameof(output));

            return ReverseStreamAsync(input, output, cancellationToken);
        }

        /// <summary>
        /// Encrypts the input stream and writes the encrypted content into the output stream.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        public void Encrypt(Stream input, Stream output, Action<IEncryptionContext>? optionsConfig = null)
        {
            Requires.NotNull(input, nameof(input));
            Requires.NotNull(output, nameof(output));

            ReverseStream(input, output);
        }

        /// <summary>
        /// Decrypts the input stream and writes the decrypted content into the output stream.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        public void Decrypt(Stream input, Stream output, Action<IEncryptionContext>? optionsConfig = null)
        {
            Requires.NotNull(input, nameof(input));
            Requires.NotNull(output, nameof(output));

            ReverseStream(input, output);
        }

        /// <summary>
        /// Reverse stream asynchronously.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        private static async Task ReverseStreamAsync(Stream input, Stream output, CancellationToken cancellationToken)
        {
            var inputBuffer = new byte[1000];
            var outputBuffer = new byte[1000];

            var count = await input.ReadAsync(inputBuffer, 0, 1000, cancellationToken).PreserveThreadContext();
            while (count > 0)
            {
                for (var i = 0; i < count; i++)
                {
                    outputBuffer[i] = inputBuffer[count - 1 - i];
                }

                await output.WriteAsync(outputBuffer, 0, count, cancellationToken).PreserveThreadContext();
                count = await input.ReadAsync(inputBuffer, 0, 1000, cancellationToken).PreserveThreadContext();
            }
        }

        /// <summary>
        /// Reverse stream asynchronously.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="output">The output stream.</param>
        private static void ReverseStream(Stream input, Stream output)
        {
            var inputBuffer = new byte[1000];
            var outputBuffer = new byte[1000];

            var count = input.Read(inputBuffer, 0, 1000);
            while (count > 0)
            {
                for (var i = 0; i < count; i++)
                {
                    outputBuffer[i] = inputBuffer[count - 1 - i];
                }

                output.Write(outputBuffer, 0, count);
                count = input.Read(inputBuffer, 0, 1000);
            }
        }
    }
}