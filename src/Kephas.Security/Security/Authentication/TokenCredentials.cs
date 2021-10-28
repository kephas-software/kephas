// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TokenCredentials.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the token authentication context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authentication
{
    using Kephas.Dynamic;

    /// <summary>
    /// Token based credentials.
    /// </summary>
    public class TokenCredentials : Expando, ICredentials
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenCredentials"/> class.
        /// </summary>
        /// <param name="token">The token.</param>
        public TokenCredentials(string token)
        {
            this.Token = token;
        }

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public string Token { get; }
    }
}