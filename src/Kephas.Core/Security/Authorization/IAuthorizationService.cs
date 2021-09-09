﻿// --------------------------------------------------------------------------------------------------------------------
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
    using System;
    using System.Collections.Generic;
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
        /// Query asynchronously whether the authorization context has the requested permissions.
        /// </summary>
        /// <param name="executionContext">The context for the execution to be authorized.</param>
        /// <param name="permissions">The permissions.</param>
        /// <param name="scope">Optional. The authorization scope.</param>
        /// <param name="authConfig">Optional. The authorization configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result returning true if permission is granted, false if not.
        /// </returns>
        Task<bool> AuthorizeAsync(
            IContext executionContext,
            IEnumerable<object> permissions,
            object? scope = null,
            Action<IAuthorizationContext>? authConfig = null,
            CancellationToken cancellationToken = default);

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
            object? scope = null,
            Action<IAuthorizationContext>? authConfig = null)
        {
            return this.AuthorizeAsync(executionContext, permissions, authConfig).GetResultNonLocking();
        }
    }
}