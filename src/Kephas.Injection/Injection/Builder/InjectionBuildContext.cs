// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionBuildContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the injector builder context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Security.Principal;

    using Kephas.Dynamic;
    using Kephas.Injection.Configuration;
    using Kephas.Logging;
    using Kephas.Services;

    /// <summary>
    /// A context for building the injector.
    /// </summary>
    public class InjectionBuildContext : Expando, IInjectionBuildContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InjectionBuildContext"/> class.
        /// </summary>
        /// <param name="assemblies">
        /// An enumeration of assemblies used in injection.
        /// </param>
        public InjectionBuildContext(IEnumerable<Assembly>? assemblies = null)
        {
            this.Assemblies = assemblies is null
                ? new List<Assembly>()
                : new List<Assembly>(assemblies);
        }

        /// <summary>
        /// Gets the application service information providers.
        /// </summary>
        /// <value>
        /// The application service information providers.
        /// </value>
        public ICollection<IAppServiceInfosProvider> AppServiceInfosProviders { get; } = new List<IAppServiceInfosProvider>();

        /// <summary>
        /// Gets the list of assemblies used in injection.
        /// </summary>
        public ICollection<Assembly> Assemblies { get; }

        /// <summary>
        /// Gets the injection settings.
        /// </summary>
        public InjectionSettings Settings { get; } = new ();

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger? Logger { get; set; }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        void IDisposable.Dispose()
        {
        }
    }
}