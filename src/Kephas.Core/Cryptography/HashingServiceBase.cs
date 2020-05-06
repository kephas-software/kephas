// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashingServiceBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
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

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// A hashing service base.
    /// </summary>
    public abstract class HashingServiceBase : IHashingService
    {
        private readonly IContextFactory contextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="HashingServiceBase"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        public HashingServiceBase(IContextFactory contextFactory)
        {
            Requires.NotNull(contextFactory, nameof(contextFactory));

            this.contextFactory = contextFactory;
        }

        /// <summary>
        /// Hashes the value with the optionally provided string.
        /// </summary>
        /// <param name="value">The value to be hashed.</param>
        /// <param name="optionsConfig">Optional. Function for hashing options configuration.</param>
        /// <returns>
        /// The hashed value.
        /// </returns>
        public virtual byte[]? Hash(byte[] value, Action<IHashingContext>? optionsConfig = null)
        {
            if (value == null)
            {
                return null;
            }

            var hashingContext = this.GetHashingContext(optionsConfig);

            try
            {
                var hasher = this.CreateHashAlgorithm(hashingContext);
                var hash = hasher.ComputeHash(this.GetSaltedValue(value, hashingContext?.Salt));
                return hash;
            }
            finally
            {
                hashingContext?.Dispose();
            }
        }

        /// <summary>
        /// Gets the hashing context.
        /// </summary>
        /// <remarks>
        /// If a configuration is provided, a new <see cref="HashingContext"/> is created and the configuration applied,
        /// otherwise <c>null</c> is returned.
        /// </remarks>
        /// <param name="optionsConfig">Function for hashing options configuration.</param>
        /// <returns>
        /// The hashing context.
        /// </returns>
        protected virtual IHashingContext? GetHashingContext(Action<IHashingContext>? optionsConfig)
        {
            if (optionsConfig == null)
            {
                return null;
            }

            var context = this.contextFactory.CreateContext<HashingContext>();
            optionsConfig(context);
            return context;
        }

        /// <summary>
        /// Creates the hash algorithm.
        /// </summary>
        /// <param name="context">The hashing context.</param>
        /// <returns>
        /// The new hash algorithm.
        /// </returns>
        protected abstract HashAlgorithm CreateHashAlgorithm(IHashingContext? context = null);

        /// <summary>
        /// Gets salted value.
        /// </summary>
        /// <param name="value">The value to be hashed.</param>
        /// <param name="saltBytes">The salt bytes (optional).</param>
        /// <returns>
        /// The salted value.
        /// </returns>
        protected virtual byte[] GetSaltedValue(byte[] value, byte[]? saltBytes)
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