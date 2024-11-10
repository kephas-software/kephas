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
    using System;

    using Kephas.Services;

    /// <summary>
    /// An authentication context.
    /// </summary>
    public class AuthenticationContext : Context, IAuthenticationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationContext"/> class.
        /// </summary>
        /// <param name="serviceProvider">The injector.</param>
        public AuthenticationContext(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationContext"/> class.
        /// </summary>
        /// <param name="serviceProvider">The injector.</param>
        /// <param name="credentials">The credentials.</param>
        public AuthenticationContext(IServiceProvider serviceProvider, ICredentials credentials)
            : base(serviceProvider)
        {
            this.Credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
        }

        /// <summary>
        /// Gets or sets a value indicating whether to throw on failure.
        /// </summary>
        /// <value>
        /// True if throw on failure, false if not.
        /// </value>
        public bool ThrowOnFailure { get; set; } = true;

        /// <summary>
        /// Gets or sets the credentials.
        /// </summary>
        /// <value>
        /// The credentials.
        /// </value>
        public ICredentials? Credentials { get; set; }
    }
}