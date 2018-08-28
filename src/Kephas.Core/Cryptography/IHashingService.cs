// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHashingService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IHashingService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Cryptography
{
    using System.Text;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Application service contract for hashing.
    /// </summary>
    [SharedAppServiceContract]
    public interface IHashingService
    {
        /// <summary>
        /// Hashes the value with the optionally provided string.
        /// </summary>
        /// <param name="value">The value to be hashed.</param>
        /// <param name="hashingContext">Optional. Context for the hashing.</param>
        /// <returns>
        /// The hashed value.
        /// </returns>
        byte[] Hash(byte[] value, IHashingContext hashingContext = null);
    }

    /// <summary>
    /// Extension methods for <see cref="IHashingService"/>.
    /// </summary>
    public static class HashingServiceExtensions
    {
        /// <summary>
        /// Hashes the value with the optionally provided string.
        /// </summary>
        /// <param name="hashingService">The hashingService to act on.</param>
        /// <param name="value">The value to be hashed.</param>
        /// <param name="hashingContext">Optional. Context for the hashing.</param>
        /// <returns>
        /// The hashed value.
        /// </returns>
        public static byte[] Hash(this IHashingService hashingService, string value, IHashingContext hashingContext = null)
        {
            Requires.NotNull(hashingService, nameof(hashingService));

            var valueBytes = string.IsNullOrEmpty(value) ? null : Encoding.UTF8.GetBytes(value);
            return hashingService.Hash(valueBytes, hashingContext);
        }
    }
}