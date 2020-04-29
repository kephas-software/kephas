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
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

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
        byte[] GenerateKey(Action<IEncryptionContext>? optionsConfig = null);

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
            Action<IEncryptionContext>? optionsConfig = null,
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
            Action<IEncryptionContext>? optionsConfig = null,
            CancellationToken cancellationToken = default);

#if NETSTANDARD2_1
        /// <summary>
        /// Encrypts the input stream and writes the encrypted content into the output stream.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        void Encrypt(
            Stream input,
            Stream output,
            Action<IEncryptionContext>? optionsConfig = null)
        {
            this.EncryptAsync(input, output, optionsConfig).WaitNonLocking();
        }

        /// <summary>
        /// Decrypts the input stream and writes the decrypted content into the output stream.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        void Decrypt(
            Stream input,
            Stream output,
            Action<IEncryptionContext>? optionsConfig = null)
        {
            this.DecryptAsync(input, output, optionsConfig).WaitNonLocking();
        }
#endif
    }

#if NETSTANDARD2_1
#else
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
            Action<IEncryptionContext>? optionsConfig = null);

        /// <summary>
        /// Decrypts the input stream and writes the decrypted content into the output stream.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        void Decrypt(
            Stream input,
            Stream output,
            Action<IEncryptionContext>? optionsConfig = null);
    }
#endif

    /// <summary>
    /// Extension methods for <see cref="IEncryptionService"/>.
    /// </summary>
    public static class EncryptionServiceExtensions
    {
#if NETSTANDARD2_1
#else
        /// <summary>
        /// Encrypts the input stream and writes the encrypted content into the output stream.
        /// </summary>
        /// <param name="encryptionService">The encryption service.</param>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        public static void Encrypt(
            this IEncryptionService encryptionService,
            Stream input,
            Stream output,
            Action<IEncryptionContext>? optionsConfig = null)
        {
            Requires.NotNull(encryptionService, nameof(encryptionService));

            if (encryptionService is ISyncEncryptionService syncEncryptionService)
            {
                syncEncryptionService.Encrypt(input, output, optionsConfig);
            }
            else
            {
                encryptionService.EncryptAsync(input, output, optionsConfig).WaitNonLocking();
            }
        }

        /// <summary>
        /// Decrypts the input stream and writes the decrypted content into the output stream.
        /// </summary>
        /// <param name="encryptionService">The encryption service.</param>
        /// <param name="input">The input stream.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        public static void Decrypt(
            this IEncryptionService encryptionService,
            Stream input,
            Stream output,
            Action<IEncryptionContext>? optionsConfig = null)
        {
            Requires.NotNull(encryptionService, nameof(encryptionService));

            if (encryptionService is ISyncEncryptionService syncEncryptionService)
            {
                syncEncryptionService.Decrypt(input, output, optionsConfig);
            }
            else
            {
                encryptionService.DecryptAsync(input, output, optionsConfig).WaitNonLocking();
            }
        }
#endif

        /// <summary>
        /// Encrypts the input string and returns a promise of the encrypted string (Base64 encoded).
        /// </summary>
        /// <param name="encryptionService">The encryption service to act on.</param>
        /// <param name="input">The input string.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of the encrypted bytes (Base64 encoded).
        /// </returns>
        public static async Task<string> EncryptAsync(
            this IEncryptionService encryptionService,
            string input,
            Action<IEncryptionContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            var bytes = await EncryptAsync(encryptionService, Encoding.UTF8.GetBytes(input), optionsConfig, cancellationToken)
                            .PreserveThreadContext();
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Encrypts the input bytes and returns a promise of the encrypted bytes.
        /// </summary>
        /// <param name="encryptionService">The encryption service to act on.</param>
        /// <param name="input">The input bytes.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of the encrypted bytes.
        /// </returns>
        public static async Task<byte[]> EncryptAsync(
            this IEncryptionService encryptionService,
            byte[] input,
            Action<IEncryptionContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(encryptionService, nameof(encryptionService));

            using var inputStream = new MemoryStream(input);
            using var outputStream = new MemoryStream();

            await encryptionService.EncryptAsync(inputStream, outputStream, optionsConfig, cancellationToken).PreserveThreadContext();
            var outputBytes = outputStream.ToArray();
            return outputBytes;
        }

        /// <summary>
        /// Encrypts the input string and returns the encrypted string (Base64 encoded).
        /// </summary>
        /// <param name="encryptionService">The encryption service to act on.</param>
        /// <param name="input">The input string.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <returns>
        /// The Base64 encoded encrypted bytes.
        /// </returns>
        public static string Encrypt(
            this IEncryptionService encryptionService,
            string input,
            Action<IEncryptionContext>? optionsConfig = null)
        {
            var bytes = Encrypt(encryptionService, Encoding.UTF8.GetBytes(input), optionsConfig);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Encrypts the input bytes and returns the encrypted bytes.
        /// </summary>
        /// <param name="encryptionService">The encryption service to act on.</param>
        /// <param name="input">The input bytes.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <returns>
        /// The encrypted bytes.
        /// </returns>
        public static byte[] Encrypt(
            this IEncryptionService encryptionService,
            byte[] input,
            Action<IEncryptionContext>? optionsConfig = null)
        {
            Requires.NotNull(encryptionService, nameof(encryptionService));

            using var inputStream = new MemoryStream(input);
            using var outputStream = new MemoryStream();

            encryptionService.Encrypt(inputStream, outputStream, optionsConfig);
            var outputBytes = outputStream.ToArray();
            return outputBytes;
        }

        /// <summary>
        /// Decrypts the input Base64 encoded string and returns a promise of the decrypted string.
        /// </summary>
        /// <param name="encryptionService">The encryption service to act on.</param>
        /// <param name="input">The Base64 encoded input string.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of a decrypted string.
        /// </returns>
        public static async Task<string> DecryptAsync(
            this IEncryptionService encryptionService,
            string input,
            Action<IEncryptionContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            var bytes = await DecryptAsync(
                            encryptionService,
                            Convert.FromBase64String(input),
                            optionsConfig,
                            cancellationToken).PreserveThreadContext();

            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Decrypts the input bytes and returns a promise of the decrypted bytes.
        /// </summary>
        /// <param name="encryptionService">The encryption service to act on.</param>
        /// <param name="input">The input bytes.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of a decrypted bytes.
        /// </returns>
        public static async Task<byte[]> DecryptAsync(
            this IEncryptionService encryptionService,
            byte[] input,
            Action<IEncryptionContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(encryptionService, nameof(encryptionService));

            using var inputStream = new MemoryStream(input);
            using var outputStream = new MemoryStream();

            await encryptionService.DecryptAsync(inputStream, outputStream, optionsConfig, cancellationToken).PreserveThreadContext();
            var outputBytes = outputStream.ToArray();
            return outputBytes;
        }

        /// <summary>
        /// Decrypts the input string and returns the decrypted string.
        /// </summary>
        /// <param name="encryptionService">The encryption service to act on.</param>
        /// <param name="input">The Base64 encoded input string.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <returns>
        /// A promise of the decrypted string.
        /// </returns>
        public static string Decrypt(
            this IEncryptionService encryptionService,
            string input,
            Action<IEncryptionContext>? optionsConfig = null)
        {
            var bytes = Decrypt(encryptionService, Convert.FromBase64String(input), optionsConfig);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Decrypts the input bytes and returns the decrypted bytes.
        /// </summary>
        /// <param name="encryptionService">The encryption service to act on.</param>
        /// <param name="input">The input bytes.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <returns>
        /// The decrypted bytes.
        /// </returns>
        public static byte[] Decrypt(
            this IEncryptionService encryptionService,
            byte[] input,
            Action<IEncryptionContext>? optionsConfig = null)
        {
            Requires.NotNull(encryptionService, nameof(encryptionService));

            using var inputStream = new MemoryStream(input);
            using var outputStream = new MemoryStream();

            encryptionService.Decrypt(inputStream, outputStream, optionsConfig);
            var outputBytes = outputStream.ToArray();
            return outputBytes;
        }
    }
}