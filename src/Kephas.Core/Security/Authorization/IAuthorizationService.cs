// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthorizationService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAuthorizationService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Singleton application service contract for handling authorization.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IAuthorizationService
    {
        /// <summary>
        /// Authorizes the provided context asynchronously.
        /// </summary>
        /// <param name="authContext">Context for the authorization.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result returning true if permission is granted, false if not.
        /// </returns>
        Task<bool> AuthorizeAsync(
            IAuthorizationContext authContext,
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Extension methods for <see cref="IAuthorizationService"/>.
    /// </summary>
    public static class AuthorizationServiceExtensions
    {
        /// <summary>
        /// Authorizes the provided context.
        /// </summary>
        /// <param name="authorizationService">The authorization service to act on.</param>
        /// <param name="authContext">Context for the authorization.</param>
        /// <returns>
        /// An asynchronous result returning true if permission is granted, false if not.
        /// </returns>
        public static bool Authorize(this IAuthorizationService authorizationService, IAuthorizationContext authContext)
        {
            Requires.NotNull(authorizationService, nameof(authorizationService));

            if (authorizationService is ISyncAuthorizationService syncAuthorizationService)
            {
                return syncAuthorizationService.Authorize(authContext);
            }

            return authorizationService.AuthorizeAsync(authContext).GetResultNonLocking();
        }
    }
}