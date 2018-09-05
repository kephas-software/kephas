// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the authorization context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security
{
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
        public AuthorizationContext(IContext executingContext)
            : base(executingContext?.CompositionContext)
        {
            this.Identity = executingContext?.Identity;
            this.ThrowOnFailure = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to throw on failure.
        /// </summary>
        /// <value>
        /// True if throw on failure, false if not.
        /// </value>
        public bool ThrowOnFailure { get; set; }
    }
}