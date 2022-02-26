// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITokenCredentials.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authentication
{
    /// <summary>
    /// Credentials for a token.
    /// </summary>
    /// <seealso cref="ICredentials" />
    public interface ITokenCredentials : ICredentials
    {
        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public string Token { get; }
    }
}
