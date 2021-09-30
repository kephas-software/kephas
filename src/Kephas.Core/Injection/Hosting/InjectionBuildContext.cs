﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionBuildContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the injector builder context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    using Kephas.Application;
    using Kephas.Diagnostics;
    using Kephas.Injection.Configuration;
    using Kephas.Logging;
    using Kephas.Reflection;
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
        /// If not provided, the application assemblies provided by the <see cref="IAppRuntime"/> is used.
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
        /// Gets the injection settings.
        /// </summary>
        public InjectionSettings Settings { get; } = new ();
    }
}