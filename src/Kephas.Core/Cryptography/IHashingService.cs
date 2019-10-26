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
    using System.Runtime.CompilerServices;
    using System.Text;

    using Kephas.Diagnostics.Contracts;
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
        byte[] Hash(byte[] value, Action<IHashingContext> optionsConfig = null);
    }

    /// <summary>
    /// Extension methods for <see cref="IHashingService"/>.
    /// </summary>
    public static class HashingServiceExtensions
    {
        /// <summary>
        /// Hashes the value with the optionally provided string.
        /// </summary>
        /// <param name="hashingService">The hashing service.</param>
        /// <param name="value">The value to be hashed.</param>
        /// <param name="optionsConfig">Optional. Function for hashing options configuration.</param>
        /// <returns>
        /// The hashed value.
        /// </returns>
        public static byte[] Hash(this IHashingService hashingService, string value, Action<IHashingContext> optionsConfig = null)
        {
            Requires.NotNull(hashingService, nameof(hashingService));

            var valueBytes = string.IsNullOrEmpty(value) ? null : Encoding.UTF8.GetBytes(value);
            return hashingService.Hash(valueBytes, optionsConfig);
        }

        /// <summary>
        /// Hashes the value with the optionally provided string.
        /// </summary>
        /// <param name="hashingService">The hashing service.</param>
        /// <param name="value">The value to be hashed.</param>
        /// <param name="salt">The hashing salt.</param>
        /// <returns>
        /// The hashed value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Hash(this IHashingService hashingService, string value, byte[] salt)
        {
            return Hash(hashingService, value, ctx => ctx.Salt(salt));
        }

        /// <summary>
        /// Hashes the value with the optionally provided string.
        /// </summary>
        /// <param name="hashingService">The hashing service.</param>
        /// <param name="value">The value to be hashed.</param>
        /// <param name="salt">The hashing salt.</param>
        /// <returns>
        /// The hashed value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Hash(this IHashingService hashingService, byte[] value, byte[] salt)
        {
            Requires.NotNull(hashingService, nameof(hashingService));

            return hashingService.Hash(value, ctx => ctx.Salt(salt));
        }
    }
}