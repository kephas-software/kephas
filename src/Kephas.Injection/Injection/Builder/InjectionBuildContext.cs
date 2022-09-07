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

    using Kephas.Application;
    using Kephas.Injection.Configuration;
    using Kephas.Services;

    /// <summary>
    /// A context for building the injector.
    /// </summary>
    public class InjectionBuildContext : Context, IInjectionBuildContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InjectionBuildContext"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="assemblies">
        /// An enumeration of assemblies used in injection.
        /// </param>
        public InjectionBuildContext(IAmbientServices ambientServices, IList<Assembly>? assemblies = null)
            : base(ambientServices ?? throw new ArgumentNullException(nameof(ambientServices)))
        {
            this.Assemblies = assemblies ?? new List<Assembly>();
        }

        /// <summary>
        /// Gets the application service information providers.
        /// </summary>
        /// <value>
        /// The application service information providers.
        /// </value>
        public IList<IAppServiceInfosProvider> AppServiceInfosProviders { get; } = new List<IAppServiceInfosProvider>();

        /// <summary>
        /// Gets the list of assemblies used in injection.
        /// </summary>
        public IList<Assembly> Assemblies { get; }

        /// <summary>
        /// Gets the application assemblies.
        /// </summary>
        /// <returns>An enumeration of assemblies.</returns>
        public IEnumerable<Assembly> GetAppAssemblies()
        {
            return this.AmbientServices.GetAppRuntime().GetAppAssemblies();
        }

        /// <summary>
        /// Gets the injection settings.
        /// </summary>
        public InjectionSettings Settings { get; } = new ();
    }
}