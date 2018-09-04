// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullSecurityService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the null security service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security
{
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// A null security service.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullSecurityService : ISecurityService
    {
        /// <summary>
        /// Gets asynchronously the identity for the provided token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="context">The requiring context (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// An asynchronous result that yields the identity.
        /// </returns>
        public Task<IIdentity> GetIdentityAsync(string token, IContext context = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IIdentity>(null);
        }

        /// <summary>
        /// Gets asynchronously a token for the provided identity.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <param name="context">The requiring context (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// An asynchronous result that yields the token.
        /// </returns>
        public Task<string> GetTokenAsync(IIdentity identity, IContext context = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<string>(null);
        }
    }
}