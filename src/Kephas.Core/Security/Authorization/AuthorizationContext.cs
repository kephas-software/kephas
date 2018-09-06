// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the authorization context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization
{
    using System.Collections.Generic;

    using Kephas.Services;

    /// <summary>
    /// An authorization context.
    /// </summary>
    public class AuthorizationContext : Context, IAuthorizationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationContext"/> class.
        /// </summary>
        /// <param name="executingContext">Context for the executing.</param>
        /// <param name="requiredPermissions">A variable-length parameters list containing required permissions.</param>
        public AuthorizationContext(IContext executingContext, params string[] requiredPermissions)
            : this(executingContext, (IEnumerable<string>)requiredPermissions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationContext"/> class.
        /// </summary>
        /// <param name="executingContext">Context for the executing.</param>
        /// <param name="requiredPermissions">A variable-length parameters list containing required permissions.</param>
        /// <param name="scope">Optional. The authorization scope.</param>
        public AuthorizationContext(IContext executingContext, IEnumerable<string> requiredPermissions, object scope = null)
            : base(executingContext?.CompositionContext)
        {
            this.Identity = executingContext?.Identity;
            this.RequiredPermissions = requiredPermissions;
            this.Scope = scope;
        }

        /// <summary>
        /// Gets the required permissions.
        /// </summary>
        /// <value>
        /// The required permissions.
        /// </value>
        public IEnumerable<string> RequiredPermissions { get; }

        /// <summary>
        /// Gets or sets the authorization scope.
        /// </summary>
        /// <value>
        /// The scope.
        /// </value>
        public object Scope { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to throw on failure.
        /// </summary>
        /// <value>
        /// True if throw on failure, false if not.
        /// </value>
        public bool ThrowOnFailure { get; set; } = true;
    }
}