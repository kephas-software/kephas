// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationScopeContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the authorization scope context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization
{
    using System;
    using Kephas.Services;

    /// <summary>
    /// An authorization scope context.
    /// </summary>
    public class AuthorizationScopeContext : Context, IAuthorizationScopeContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationScopeContext"/> class.
        /// </summary>
        /// <param name="callingContext">Context for the calling.</param>
        public AuthorizationScopeContext(IContext callingContext)
            : base((callingContext ?? throw new ArgumentNullException(nameof(callingContext))).Injector)
        {
            this.CallingContext = callingContext;
        }

        /// <summary>
        /// Gets or sets the calling context.
        /// </summary>
        /// <value>
        /// The calling context.
        /// </value>
        public IContext CallingContext { get; set; }
    }
}
