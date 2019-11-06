// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserPasswordHashCredentials.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the password authentication context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authentication
{
    using System;

    using Kephas.Dynamic;

    /// <summary>
    /// User and password hash based credentials.
    /// </summary>
    public class UserPasswordHashCredentials : Expando, ICredentials
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserPasswordHashCredentials"/> class.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="passwordHash">The hashed user password, encoded as a Base64 string.</param>
        public UserPasswordHashCredentials(string userName, string passwordHash)
        {
            this.UserName = userName;
            this.PasswordHash = Convert.FromBase64String(passwordHash);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserPasswordHashCredentials"/> class.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="passwordHash">The hashed user password.</param>
        public UserPasswordHashCredentials(string userName, byte[] passwordHash)
        {
            this.UserName = userName;
            this.PasswordHash = passwordHash;
        }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; }

        /// <summary>
        /// Gets the hashed password.
        /// </summary>
        /// <value>
        /// The hashed password.
        /// </value>
        public byte[] PasswordHash { get; }
    }
}