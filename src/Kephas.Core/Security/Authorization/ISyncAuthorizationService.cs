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
    using System;
    using System.Collections.Generic;

    using Kephas.Services;

    /// <summary>
    /// Interface providing synchronous methods for the authorization service.
    /// </summary>
    public interface ISyncAuthorizationService
    {
        /// <summary>
        /// Query whether the authorization context has the requested permissions.
        /// </summary>
        /// <param name="executionContext">The context for the execution to be authorized.</param>
        /// <param name="permissions">The permissions.</param>
        /// <param name="scope">Optional. The authorization scope.</param>
        /// <param name="authConfig">Optional. The authorization configuration.</param>
        /// <returns>
        /// True if permission is granted, false if not.
        /// </returns>
        bool Authorize(
            IContext executionContext,
            IEnumerable<object> permissions,
            object scope = null,
            Action<IAuthorizationContext> authConfig = null);
    }
}