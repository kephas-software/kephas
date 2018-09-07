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
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Shared application service contract for handing authentication.
    /// </summary>
    [SharedAppServiceContract]
    public interface IAuthenticationService
    {
        /// <summary>
        /// Authenticates the user asynchronously.
        /// </summary>
        /// <param name="authContext">Context for the authentication.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the identity.
        /// </returns>
        Task<IIdentity> AuthenticateAsync(
            IAuthenticationContext authContext,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets asynchronously the identity for the provided token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="context">The requiring context (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// An asynchronous result that yields the identity.
        /// </returns>
        Task<IIdentity> GetIdentityAsync(
            object token,
            IContext context = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets asynchronously a token for the provided identity.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <param name="context">The requiring context (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// An asynchronous result that yields the token.
        /// </returns>
        Task<object> GetTokenAsync(
            IIdentity identity,
            IContext context = null,
            CancellationToken cancellationToken = default);
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
        /// <param name="authContext">Context for the authentication.</param>
        /// <returns>
        /// The identity.
        /// </returns>
        public static IIdentity Authenticate(
            this IAuthenticationService authenticationService,
            IAuthenticationContext authContext)
        {
            Requires.NotNull(authenticationService, nameof(authenticationService));

            if (authenticationService is ISyncAuthenticationService syncAuthenticationService)
            {
                return syncAuthenticationService.Authenticate(authContext);
            }

            return authenticationService.AuthenticateAsync(authContext).GetResultNonLocking();
        }

        /// <summary>
        /// Gets the identity for the provided token.
        /// </summary>
        /// <param name="authenticationService">The authentication service to act on.</param>
        /// <param name="token">The token.</param>
        /// <param name="context">Optional. The requiring context.</param>
        /// <returns>
        /// The identity.
        /// </returns>
        public static IIdentity GetIdentity(
            this IAuthenticationService authenticationService,
            object token,
            IContext context = null)
        {
            Requires.NotNull(authenticationService, nameof(authenticationService));

            if (authenticationService is ISyncAuthenticationService syncAuthenticationService)
            {
                return syncAuthenticationService.GetIdentity(token, context);
            }

            return authenticationService.GetIdentityAsync(token, context).GetResultNonLocking();
        }

        /// <summary>
        /// Gets a token for the provided identity.
        /// </summary>
        /// <param name="authenticationService">The authentication service to act on.</param>
        /// <param name="identity">The identity.</param>
        /// <param name="context">Optional. The requiring context.</param>
        /// <returns>
        /// The token.
        /// </returns>
        public static object GetToken(
            this IAuthenticationService authenticationService,
            IIdentity identity,
            IContext context = null)
        {
            Requires.NotNull(authenticationService, nameof(authenticationService));

            if (authenticationService is ISyncAuthenticationService syncAuthenticationService)
            {
                return syncAuthenticationService.GetToken(identity, context);
            }

            return authenticationService.GetTokenAsync(identity, context).GetResultNonLocking();
        }
    }
}