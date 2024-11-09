// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHashingService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IHashingService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Cryptography
{
    using System;
    using System.Text;

    using Kephas.Services;

    /// <summary>
    /// Application service contract for hashing.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IHashingService
    {
        /// <summary>
        /// Hashes the value with the optionally provided string.
        /// </summary>
        /// <param name="value">The value to be hashed.</param>
        /// <param name="optionsConfig">Optional. Function for hashing options configuration.</param>
        /// <returns>
        /// The hashed value.
        /// </returns>
        public byte[] Hash(byte[] value, Action<IHashingContext>? optionsConfig = null);

        /// <summary>
        /// Hashes the value with the optionally provided string.
        /// </summary>
        /// <param name="value">The value to be hashed.</param>
        /// <param name="optionsConfig">Optional. Function for hashing options configuration.</param>
        /// <returns>
        /// The hashed value.
        /// </returns>
        public byte[] Hash(string value, Action<IHashingContext>? optionsConfig = null)
        {
            value = value ?? throw new ArgumentNullException(nameof(value));

            return this.Hash(Encoding.UTF8.GetBytes(value), optionsConfig);
        }

        /// <summary>
        /// Hashes the value with the optionally provided string.
        /// </summary>
        /// <param name="value">The value to be hashed.</param>
        /// <param name="salt">The hashing salt.</param>
        /// <returns>
        /// The hashed value.
        /// </returns>
        public byte[] Hash(string value, byte[] salt)
        {
            return this.Hash(value, ctx => ctx.Salt(salt));
        }

        /// <summary>
        /// Hashes the value with the optionally provided string.
        /// </summary>
        /// <param name="value">The value to be hashed.</param>
        /// <param name="salt">The hashing salt.</param>
        /// <returns>
        /// The hashed value.
        /// </returns>
        public byte[] Hash(byte[] value, byte[] salt)
        {
            return this.Hash(value, ctx => ctx.Salt(salt));
        }
    }
}