// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullAuthenticationService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the null security service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authentication
{
    using System;
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// A null security service.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullAuthenticationService : IAuthenticationService
#if NETSTANDARD2_1
#else
        , ISyncAuthenticationService
#endif
    {
        /// <summary>
        /// Authenticates the user asynchronously.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <param name="authConfig">Optional. The authentication configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the identity.
        /// </returns>
        public Task<IIdentity?> AuthenticateAsync(
            ICredentials credentials,
            Action<IAuthenticationContext>? authConfig = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IIdentity?>(null);
        }

        /// <summary>
        /// Gets asynchronously the identity for the provided token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the identity.
        /// </returns>
        public Task<IIdentity?> GetIdentityAsync(
            object token,
            Action<IContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IIdentity?>(null);
        }

        /// <summary>
        /// Gets asynchronously a token for the provided identity.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the token.
        /// </returns>
        public Task<object?> GetTokenAsync(
            IIdentity identity,
            Action<IContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<object?>(null);
        }

        /// <summary>
        /// Authenticates the user.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <param name="authConfig">Optional. The authentication configuration.</param>
        /// <returns>
        /// The identity.
        /// </returns>
        public IIdentity? Authenticate(ICredentials credentials, Action<IAuthenticationContext>? authConfig = null)
        {
            return null;
        }

        /// <summary>
        /// Gets the identity for the provided token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <returns>
        /// The identity.
        /// </returns>
        public IIdentity? GetIdentity(object token, Action<IContext>? optionsConfig = null)
        {
            return null;
        }

        /// <summary>
        /// Gets a token for the provided identity.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <returns>
        /// The token.
        /// </returns>
        public object? GetToken(IIdentity identity, Action<IContext>? optionsConfig = null)
        {
            return null;
        }
    }
}