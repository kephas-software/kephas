﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Defines a base contract for context-dependent operations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Security.Principal;

    using Kephas.Dynamic;
    using Kephas.Injection;
    using Kephas.Logging;

    /// <summary>
    /// Defines a base contract for context-dependent operations.
    /// </summary>
    public interface IContext : IDynamic, ILoggable, IDisposable, IInjectable
    {
        /// <summary>
        /// Gets a context for the dependency injection/composition.
        /// </summary>
        /// <value>
        /// The injector.
        /// </value>
        IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets or sets the authenticated identity.
        /// </summary>
        /// <value>
        /// The authenticated identity.
        /// </value>
        IIdentity? Identity { get; set; }
    }
}