// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashingServiceBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the hashing service base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Cryptography
{
    using System;
    using System.Security.Cryptography;

    using Kephas.Services;

    /// <summary>
    /// A hashing service base.
    /// </summary>
    public abstract class HashingServiceBase : IHashingService
    {
        /// <summary>
        /// Hashes the value with the optionally provided string.
        /// </summary>
        /// <param name="value">The value to be hashed.</param>
        /// <param name="hashingContext">Optional. Context for the hashing.</param>
        /// <returns>
        /// The hashed value.
        /// </returns>
        public virtual byte[] Hash(byte[] value, IHashingContext hashingContext = null)
        {
            if (value == null)
            {
                return null;
            }

            var hasher = this.CreateHashAlgorithm(hashingContext);
            var hash = hasher.ComputeHash(this.GetSaltedValue(value, hashingContext?.Salt));
            return hash;
        }

        /// <summary>
        /// Creates the hash algorithm.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The new hash algorithm.
        /// </returns>
        protected abstract HashAlgorithm CreateHashAlgorithm(IContext context);

        /// <summary>
        /// Gets salted value.
        /// </summary>
        /// <param name="value">The value to be hashed.</param>
        /// <param name="saltBytes">The salt bytes (optional).</param>
        /// <returns>
        /// The salted value.
        /// </returns>
        protected virtual byte[] GetSaltedValue(byte[] value, byte[] saltBytes)
        {
            if (saltBytes == null || saltBytes.Length == 0)
            {
                return value;
            }

            var minLength = value.Length > saltBytes.Length ? saltBytes.Length : value.Length;
            var saltedValue = new byte[value.Length];

            Buffer.BlockCopy(value, 0, saltedValue, 0, value.Length);
            for (var i = 0; i < minLength; i++)
            {
                saltedValue[i] = unchecked((byte)(saltedValue[i] + saltBytes[i]));
            }

            return saltedValue;
        }
    }
}