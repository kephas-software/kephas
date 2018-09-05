// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the authentication context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authentication
{
    using Kephas.Composition;
    using Kephas.Services;

    /// <summary>
    /// An authentication context.
    /// </summary>
    public class AuthenticationContext : Context, IAuthenticationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationContext"/> class.
        /// </summary>
        /// <param name="compositionContext">Optional. The context for the composition. If not provided,
        /// <see cref="M:AmbientServices.Instance.CompositionContainer"/> will be considered.
        /// </param>
        public AuthenticationContext(ICompositionContext compositionContext = null)
            : base(compositionContext)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether to throw on failure.
        /// </summary>
        /// <value>
        /// True if throw on failure, false if not.
        /// </value>
        public bool ThrowOnFailure { get; set; } = true;
    }
}