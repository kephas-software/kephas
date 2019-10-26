// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHashingContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IHashingContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Cryptography
{
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Interface for hashing context.
    /// </summary>
    public interface IHashingContext : IContext
    {
        /// <summary>
        /// Gets or sets the salt.
        /// </summary>
        /// <value>
        /// The salt.
        /// </value>
        byte[] Salt { get; set; }
    }

    /// <summary>
    /// Extension methods for <see cref="IHashingContext"/>.
    /// </summary>
    public static class HashingContextExtensions
    {
        /// <summary>
        /// Sets the salt.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="hashingContext">The hashing context.</param>
        /// <param name="salt">The salt.</param>
        /// <returns>
        /// This <paramref name="hashingContext"/>.
        /// </returns>
        public static TContext Salt<TContext>(this TContext hashingContext, byte[] salt)
            where TContext : class, IHashingContext
        {
            Requires.NotNull(hashingContext, nameof(hashingContext));
            Requires.NotNull(salt, nameof(salt));

            hashingContext.Salt(salt);

            return hashingContext;
        }
    }
}