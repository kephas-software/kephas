// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SymmetricEncryptionServiceBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the symmetric encryption service base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Cryptography
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.IO;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A symmetric encryption service base.
    /// </summary>
    public abstract class SymmetricEncryptionServiceBase : IEncryptionService, ISyncEncryptionService
    {
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
        public Task EncryptAsync(
            Stream input,
            Stream output,
            IEncryptionContext context = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(input, nameof(input));
            Requires.NotNull(output, nameof(output));

            var algorithm = this.CreateSymmetricAlgorithm(context);
            return this.EncryptAsync(input, output, algorithm, context, cancellationToken);
        }

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
        public Task DecryptAsync(
            Stream input,
            Stream output,
            IEncryptionContext context = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(input, nameof(input));
            Requires.NotNull(output, nameof(output));

            var algorithm = this.CreateSymmetricAlgorithm(context);
            return this.DecryptAsync(input, output, algorithm, context, cancellationToken);
        }

        /// <summary>
        /// Encrypts the input stream and writes the encrypted content into the output stream.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="context">The encryption context (optional).</param>
        public void Encrypt(Stream input, Stream output, IEncryptionContext context = null)
        {
            Requires.NotNull(input, nameof(input));
            Requires.NotNull(output, nameof(output));

            var algorithm = this.CreateSymmetricAlgorithm(context);
            this.Encrypt(input, output, algorithm, context);
        }

        /// <summary>
        /// Decrypts the input stream and writes the decrypted content into the output stream.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="context">The encryption context (optional).</param>
        public void Decrypt(Stream input, Stream output, IEncryptionContext context = null)
        {
            Requires.NotNull(input, nameof(input));
            Requires.NotNull(output, nameof(output));

            var algorithm = this.CreateSymmetricAlgorithm(context);
            this.Decrypt(input, output, algorithm, context);
        }

        /// <summary>
        /// Encrypts the stream content.
        /// </summary>
        /// <param name="input">The input stream to encrypt.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="encryptionContext">Context for the encryption.</param>
        protected virtual void Encrypt(Stream input, Stream output, SymmetricAlgorithm algorithm, IEncryptionContext encryptionContext)
        {
            // generate the IV before creating the encryptor
            algorithm.GenerateIV();
            this.WriteIV(output, algorithm.IV);

            using (var crypto = new CryptoStream(output, algorithm.CreateEncryptor(), CryptoStreamMode.Write))
            {
                var bytes = input.ReadAllBytes();

                crypto.Write(bytes, 0, bytes.Length);
                crypto.FlushFinalBlock();
            }
        }

        /// <summary>
        /// Encrypts the stream content asynchronously.
        /// </summary>
        /// <param name="input">The input stream to encrypt.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="encryptionContext">Context for the encryption.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual async Task EncryptAsync(Stream input, Stream output, SymmetricAlgorithm algorithm, IEncryptionContext encryptionContext, CancellationToken cancellationToken)
        {
            // generate the IV before creating the encryptor
            algorithm.GenerateIV();
            this.WriteIV(output, algorithm.IV);

            using (var crypto = new CryptoStream(output, algorithm.CreateEncryptor(), CryptoStreamMode.Write))
            {
                var bytes = await input.ReadAllBytesAsync(cancellationToken: cancellationToken).PreserveThreadContext();

                await crypto.WriteAsync(bytes, 0, bytes.Length, cancellationToken).PreserveThreadContext();
                crypto.FlushFinalBlock();
            }
        }

        /// <summary>
        /// Decrypts the stream content.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="encryptionContext">Context for the encryption.</param>
        protected virtual void Decrypt(Stream input, Stream output, SymmetricAlgorithm algorithm, IEncryptionContext encryptionContext)
        {
            var iv = this.ReadIV(input);
            algorithm.IV = iv;

            using (var crypto = new CryptoStream(output, algorithm.CreateDecryptor(), CryptoStreamMode.Write))
            {
                var encryptedBytes = input.ReadAllBytes();
                crypto.Write(encryptedBytes, 0, encryptedBytes.Length);
                crypto.FlushFinalBlock();
            }
        }

        /// <summary>
        /// Decrypts the stream content asynchronously.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="encryptionContext">Context for the encryption.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual async Task DecryptAsync(Stream input, Stream output, SymmetricAlgorithm algorithm, IEncryptionContext encryptionContext, CancellationToken cancellationToken)
        {
            var iv = this.ReadIV(input);
            algorithm.IV = iv;

            using (var crypto = new CryptoStream(output, algorithm.CreateDecryptor(), CryptoStreamMode.Write))
            {
                var encryptedBytes = await input.ReadAllBytesAsync(cancellationToken: cancellationToken).PreserveThreadContext();
                await crypto.WriteAsync(encryptedBytes, 0, encryptedBytes.Length, cancellationToken).PreserveThreadContext();
                crypto.FlushFinalBlock();
            }
        }

        /// <summary>
        /// Creates the symmetric algorithm.
        /// </summary>
        /// <param name="encryptionContext">Context for the encryption.</param>
        /// <returns>
        /// The new symmetric algorithm.
        /// </returns>
        protected abstract SymmetricAlgorithm CreateSymmetricAlgorithm(IEncryptionContext encryptionContext);

        /// <summary>
        /// Gets mismatched key size encryption exception.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="keySize">Size of the key.</param>
        /// <returns>
        /// The mismatched key size encryption exception.
        /// </returns>
        protected virtual CryptographicException GetMismatchedKeySizeEncryptionException(SymmetricAlgorithm algorithm, int keySize)
        {
            // TODO localization
            var exceptionMessageBuilder = new StringBuilder();
            exceptionMessageBuilder
                .Append($"The provided key size - {keySize} bits - is not valid for this algorithm.")
                .Append(Environment.NewLine).Append("Valid key sizes: ");
            var validKeySizes = algorithm.LegalKeySizes;
            foreach (var keySizes in validKeySizes)
            {
                exceptionMessageBuilder.Append(Environment.NewLine).Append(
                    $"Min: {keySizes.MinSize}, Max: {keySizes.MaxSize}, Step: {keySizes.SkipSize}");
            }

            return new CryptographicException(exceptionMessageBuilder.ToString());
        }

        private byte[] ReadIV(Stream input)
        {
            var ivLength = input.ReadByte();
            var iv = new byte[ivLength];
            input.Read(iv, 0, ivLength);
            return iv;
        }

        private void WriteIV(Stream output, byte[] iv)
        {
            output.WriteByte((byte)iv.Length);
            output.Write(iv, 0, iv.Length);
        }
    }
}