// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISyncAuthorizationService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ISyncAuthorizationService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization
{
    /// <summary>
    /// Interface for synchronous authorization service.
    /// </summary>
    public interface ISyncAuthorizationService
    {
        /// <summary>
        /// Query if the authorization context has the requested permission.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// True if permission is granted, false if not.
        /// </returns>
        bool Authorize(IAuthorizationContext context);
    }
}