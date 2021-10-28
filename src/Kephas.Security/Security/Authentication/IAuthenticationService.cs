// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthenticationService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAuthenticationService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authentication
{
    using System;
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Singleton application service contract for handing authentication.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IAuthenticationService
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
        Task<IIdentity?> AuthenticateAsync(
            ICredentials credentials,
            Action<IAuthenticationContext>? authConfig = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets asynchronously the identity for the provided token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the identity.
        /// </returns>
        Task<IIdentity?> GetIdentityAsync(
            object token,
            Action<IAuthenticationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets asynchronously a token for the provided identity.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the token.
        /// </returns>
        Task<object?> GetTokenAsync(
            IIdentity identity,
            Action<IAuthenticationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Authenticates the user.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <param name="authConfig">Optional. The authentication configuration.</param>
        /// <returns>
        /// The identity.
        /// </returns>
        IIdentity? Authenticate(ICredentials credentials, Action<IAuthenticationContext>? authConfig = null)
        {
            return this.AuthenticateAsync(credentials, authConfig).GetResultNonLocking();
        }

        /// <summary>
        /// Gets the identity for the provided token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <returns>
        /// The identity.
        /// </returns>
        IIdentity? GetIdentity(object token, Action<IAuthenticationContext>? optionsConfig = null)
        {
            return this.GetIdentityAsync(token, optionsConfig).GetResultNonLocking();
        }

        /// <summary>
        /// Gets a token for the provided identity.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <returns>
        /// The token.
        /// </returns>
        object? GetToken(IIdentity identity, Action<IAuthenticationContext>? optionsConfig = null)
        {
            return this.GetTokenAsync(identity, optionsConfig).GetResultNonLocking();
        }
    }
}