// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISyncAuthenticationService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ISyncAuthenticationService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authentication
{
    using System.Security.Principal;

    using Kephas.Services;

    /// <summary>
    /// Interface for a synchronous authentication service.
    /// </summary>
    public interface ISyncAuthenticationService
    {
        /// <summary>
        /// Authenticates the user.
        /// </summary>
        /// <param name="authContext">Context for the authentication.</param>
        /// <returns>
        /// The identity.
        /// </returns>
        IIdentity Authenticate(IAuthenticationContext authContext);

        /// <summary>
        /// Gets the identity for the provided token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="context">The requiring context (optional).</param>
        /// <returns>
        /// The identity.
        /// </returns>
        IIdentity GetIdentity(object token, IContext context = null);

        /// <summary>
        /// Gets a token for the provided identity.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <param name="context">The requiring context (optional).</param>
        /// <returns>
        /// The token.
        /// </returns>
        object GetToken(IIdentity identity, IContext context = null);
    }
}