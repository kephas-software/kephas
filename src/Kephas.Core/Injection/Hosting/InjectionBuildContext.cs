// --------------------------------------------------------------------------------------------------------------------
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

    using Kephas.Diagnostics.Contracts;
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
        public InjectionBuildContext(IAmbientServices ambientServices)
            : base(ambientServices)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
        }

        /// <summary>
        /// Gets or sets the parts.
        /// </summary>
        /// <value>
        /// The parts.
        /// </value>
        public IEnumerable<Type>? Parts { get; set; }

        /// <summary>
        /// Gets or sets the application service information providers.
        /// </summary>
        /// <value>
        /// The application service information providers.
        /// </value>
        public IEnumerable<IAppServiceInfosProvider>? AppServiceInfosProviders { get; set; }
    }
}