// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TokenAuthenticationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the token authentication context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security
{
    using Kephas.Composition;

    /// <summary>
    /// A token authentication context.
    /// </summary>
    public class TokenAuthenticationContext : AuthenticationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenAuthenticationContext"/> class.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="compositionContext">
        /// Optional. The context for the composition. If not provided,
        /// <see cref="M:AmbientServices.Instance.CompositionContainer"/>
        /// will be considered.
        /// </param>
        public TokenAuthenticationContext(string token, ICompositionContext compositionContext = null)
            : base(compositionContext)
        {
            this.Token = token;
        }

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public string Token { get; }
    }
}