// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISecurityService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ISecurityService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authentication
{
    using System;
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
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
        Task<IIdentity> AuthenticateAsync(
            ICredentials credentials,
            Action<IAuthenticationContext> authConfig = null,
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
        Task<IIdentity> GetIdentityAsync(
            object token,
            Action<IContext> optionsConfig = null,
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
        Task<object> GetTokenAsync(
            IIdentity identity,
            Action<IContext> optionsConfig = null,
            CancellationToken cancellationToken = default);

#if NETSTANDARD2_1
        /// <summary>
        /// Authenticates the user.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <param name="authConfig">Optional. The authentication configuration.</param>
        /// <returns>
        /// The identity.
        /// </returns>
        IIdentity Authenticate(ICredentials credentials, Action<IAuthenticationContext> authConfig = null)
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
        IIdentity GetIdentity(object token, Action<IContext> optionsConfig = null)
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
        object GetToken(IIdentity identity, Action<IContext> optionsConfig = null)
        {
            return this.GetTokenAsync(identity, optionsConfig).GetResultNonLocking();
        }
#endif
    }

#if NETSTANDARD2_1
#else
    /// <summary>
    /// Interface for a synchronous authentication service.
    /// </summary>
    public interface ISyncAuthenticationService
    {
        /// <summary>
        /// Authenticates the user.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <param name="authConfig">Optional. The authentication configuration.</param>
        /// <returns>
        /// The identity.
        /// </returns>
        IIdentity Authenticate(ICredentials credentials, Action<IAuthenticationContext> authConfig = null);

        /// <summary>
        /// Gets the identity for the provided token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <returns>
        /// The identity.
        /// </returns>
        IIdentity GetIdentity(object token, Action<IContext> optionsConfig = null);

        /// <summary>
        /// Gets a token for the provided identity.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <returns>
        /// The token.
        /// </returns>
        object GetToken(IIdentity identity, Action<IContext> optionsConfig = null);
    }

    /// <summary>
    /// An authentication service extensions.
    /// </summary>
    public static class AuthenticationServiceExtensions
    {
        /// <summary>
        /// Authenticates the user.
        /// </summary>
        /// <param name="authenticationService">The authentication service to act on.</param>
        /// <param name="credentials">The credentials.</param>
        /// <param name="authConfig">The authentication configuration.</param>
        /// <returns>
        /// The identity.
        /// </returns>
        public static IIdentity Authenticate(
            this IAuthenticationService authenticationService,
            ICredentials credentials,
            Action<IAuthenticationContext> authConfig)
        {
            Requires.NotNull(authenticationService, nameof(authenticationService));

            if (authenticationService is ISyncAuthenticationService syncAuthenticationService)
            {
                return syncAuthenticationService.Authenticate(credentials, authConfig);
            }

            return authenticationService.AuthenticateAsync(credentials, authConfig).GetResultNonLocking();
        }

        /// <summary>
        /// Gets the identity for the provided token.
        /// </summary>
        /// <param name="authenticationService">The authentication service to act on.</param>
        /// <param name="token">The token.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <returns>
        /// The identity.
        /// </returns>
        public static IIdentity GetIdentity(
            this IAuthenticationService authenticationService,
            object token,
            Action<IContext> optionsConfig = null)
        {
            Requires.NotNull(authenticationService, nameof(authenticationService));

            if (authenticationService is ISyncAuthenticationService syncAuthenticationService)
            {
                return syncAuthenticationService.GetIdentity(token, optionsConfig);
            }

            return authenticationService.GetIdentityAsync(token, optionsConfig).GetResultNonLocking();
        }

        /// <summary>
        /// Gets a token for the provided identity.
        /// </summary>
        /// <param name="authenticationService">The authentication service to act on.</param>
        /// <param name="identity">The identity.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <returns>
        /// The token.
        /// </returns>
        public static object GetToken(
            this IAuthenticationService authenticationService,
            IIdentity identity,
            Action<IContext> optionsConfig = null)
        {
            Requires.NotNull(authenticationService, nameof(authenticationService));

            if (authenticationService is ISyncAuthenticationService syncAuthenticationService)
            {
                return syncAuthenticationService.GetToken(identity, optionsConfig);
            }

            return authenticationService.GetTokenAsync(identity, optionsConfig).GetResultNonLocking();
        }
    }
#endif

}