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
    using System;
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
}