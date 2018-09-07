﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullSecurityService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the null security service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authentication
{
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// A null security service.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullAuthenticationService : IAuthenticationService, ISyncAuthenticationService
    {
        /// <summary>
        /// Authenticates the user asynchronously.
        /// </summary>
        /// <param name="authContext">Context for the authentication.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the identity.
        /// </returns>
        public Task<IIdentity> AuthenticateAsync(IAuthenticationContext authContext, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IIdentity>(null);
        }

        /// <summary>
        /// Gets asynchronously the identity for the provided token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="context">The requiring context (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// An asynchronous result that yields the identity.
        /// </returns>
        public Task<IIdentity> GetIdentityAsync(
            object token,
            IContext context = null,
            CancellationToken cancellationToken = default)
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
        public Task<object> GetTokenAsync(
            IIdentity identity,
            IContext context = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Authenticates the user.
        /// </summary>
        /// <param name="authContext">Context for the authentication.</param>
        /// <returns>
        /// The identity.
        /// </returns>
        public IIdentity Authenticate(IAuthenticationContext authContext)
        {
            return null;
        }

        /// <summary>
        /// Gets the identity for the provided token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="context">The requiring context (optional).</param>
        /// <returns>
        /// The identity.
        /// </returns>
        public IIdentity GetIdentity(object token, IContext context = null)
        {
            return null;
        }

        /// <summary>
        /// Gets a token for the provided identity.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <param name="context">The requiring context (optional).</param>
        /// <returns>
        /// The token.
        /// </returns>
        public object GetToken(IIdentity identity, IContext context = null)
        {
            return null;
        }
    }
}