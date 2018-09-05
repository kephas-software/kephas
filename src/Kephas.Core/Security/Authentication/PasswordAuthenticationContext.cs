// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PasswordAuthenticationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the password authentication context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authentication
{
    using Kephas.Composition;

    /// <summary>
    /// A password authentication context.
    /// </summary>
    public class PasswordAuthenticationContext : AuthenticationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordAuthenticationContext"/> class.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="compositionContext">
        /// Optional. The context for the composition. If not provided,
        /// <see cref="M:AmbientServices.Instance.CompositionContainer"/> will be considered.
        /// </param>
        public PasswordAuthenticationContext(string userName, string password, ICompositionContext compositionContext = null)
            : base(compositionContext)
        {
            this.UserName = userName;
            this.Password = password;
        }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; }

        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; }
    }
}