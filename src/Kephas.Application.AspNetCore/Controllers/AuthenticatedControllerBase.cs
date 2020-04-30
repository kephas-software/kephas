// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticatedControllerBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.AspNetCore.Controllers
{
    using System;
    using System.Security.Authentication;
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Security.Authentication;
    using Kephas.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Base class for authenticated controllers.
    /// </summary>
    public abstract class AuthenticatedControllerBase : Controller, ILoggable
    {
        private Lazy<ILogger> lazyLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticatedControllerBase"/> class.
        /// </summary>
        /// <param name="authenticationService">The authentication service.</param>
        /// <param name="logManager">Optional. manager for log.</param>
        protected AuthenticatedControllerBase(
            IAuthenticationService authenticationService,
            ILogManager? logManager = null)
        {
            this.lazyLogger = new Lazy<ILogger>(
                () => logManager?.GetLogger(this.GetType())
                        ?? this.GetLogger(null));
            this.AuthenticationService = authenticationService;
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger Logger
        {
            get => this.lazyLogger.Value;
            protected internal set
            {
                Requires.NotNull(value, nameof(value));
                this.lazyLogger = new Lazy<ILogger>(() => value);
            }
        }

        /// <summary>
        /// Gets the authentication service.
        /// </summary>
        /// <value>
        /// The authentication service.
        /// </value>
        protected IAuthenticationService AuthenticationService { get; }

        /// <summary>
        /// Gets the session identity.
        /// </summary>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The session identity.
        /// </returns>
        protected async Task<IIdentity?> GetSessionIdentityAsync(CancellationToken cancellationToken = default)
        {
            if (this.User == null)
            {
                return null;
            }

            var identity = await this.AuthenticationService.GetIdentityAsync(this.User, cancellationToken: cancellationToken)
                               .PreserveThreadContext();
            if (identity?.Name != this.User.Identity.Name)
            {
                // TODO localization
                throw new AuthenticationException($"The user changed since last login, please logout and login again.");
            }

            return identity;
        }
    }
}
