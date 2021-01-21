// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PasswordHasher.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4
{
    using System;

    using Kephas.Cryptography;
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// Password hasher to use the hashing service.
    /// </summary>
    /// <typeparam name="TUser">The user type.</typeparam>
    /// <seealso cref="Microsoft.AspNetCore.Identity.IPasswordHasher{TUser}" />
    public class PasswordHasher<TUser> : IPasswordHasher<TUser>
        where TUser : class
    {
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordHasher{TUser}"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public PasswordHasher(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Returns a hashed representation of the supplied <paramref name="password" /> for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose password is to be hashed.</param>
        /// <param name="password">The password to hash.</param>
        /// <returns>
        /// A hashed representation of the supplied <paramref name="password" /> for the specified <paramref name="user" />.
        /// </returns>
        public string HashPassword(TUser user, string password) => this.Hash(password);

        /// <summary>
        /// Returns a <see cref="T:Microsoft.AspNetCore.Identity.PasswordVerificationResult" /> indicating the result of a password hash comparison.
        /// </summary>
        /// <param name="user">The user whose password should be verified.</param>
        /// <param name="hashedPassword">The hash value for a user's stored password.</param>
        /// <param name="providedPassword">The password supplied for comparison.</param>
        /// <returns>
        /// A <see cref="T:Microsoft.AspNetCore.Identity.PasswordVerificationResult" /> indicating the result of a password hash comparison.
        /// </returns>
        /// <remarks>
        /// Implementations of this method should be time consistent.
        /// </remarks>
        public PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
        {
            if (providedPassword == hashedPassword)
            {
                return PasswordVerificationResult.Success;
            }

            var hash = this.Hash(providedPassword);
            if (hash == hashedPassword)
            {
                return PasswordVerificationResult.Success;
            }

            return PasswordVerificationResult.Failed;
        }

        private string Hash(string value)
        {
            var hasher = this.serviceProvider.GetRequiredService<IHashingService>();
            var hash = hasher.Hash(value);
            return Convert.ToBase64String(hash);
        }
    }
}