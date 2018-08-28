// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AesEncryptionService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the aes encryption service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Cryptography
{
    using System.Security.Cryptography;

    using Kephas.Services;

    /// <summary>
    /// The AES encryption service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class AesEncryptionService : SymmetricEncryptionServiceBase
    {
        /// <summary>
        /// Gets the key size.
        /// </summary>
        /// <param name="encryptionContext">Context for the encryption.</param>
        /// <returns>
        /// The key size.
        /// </returns>
        protected virtual int GetKeySize(IEncryptionContext encryptionContext)
        {
            return 256;
        }

        /// <summary>
        /// Gets the encryption/decryption key.
        /// </summary>
        /// <param name="encryptionContext">Context for the encryption.</param>
        /// <returns>
        /// An array of byte.
        /// </returns>
        protected virtual byte[] GetKey(IEncryptionContext encryptionContext)
        {
            return encryptionContext.Key;
        }

        /// <summary>
        /// Creates the symmetric algorithm.
        /// </summary>
        /// <param name="encryptionContext">Context for the encryption.</param>
        /// <returns>
        /// The new symmetric algorithm.
        /// </returns>
        protected override SymmetricAlgorithm CreateSymmetricAlgorithm(IEncryptionContext encryptionContext)
        {
            var algorithm = new AesManaged();

            var keySize = this.GetKeySize(encryptionContext);
            if (!algorithm.ValidKeySize(keySize))
            {
                throw this.GetMismatchedKeySizeEncryptionException(algorithm, keySize);
            }

            algorithm.KeySize = keySize;
            algorithm.Padding = PaddingMode.PKCS7;

            // set the key after setting the key size, otherwise the Key will be overwritten with a new generated value.
            algorithm.Key = this.GetKey(encryptionContext);

            return algorithm;
        }
    }
}