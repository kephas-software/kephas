// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionRegistrationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the composition container builder context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Hosting
{
    using System;
    using System.Collections.Generic;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection.Conventions;
    using Kephas.Services;

    /// <summary>
    /// A composition container builder context.
    /// </summary>
    public class InjectionRegistrationContext : Context, IInjectionRegistrationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InjectionRegistrationContext"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        public InjectionRegistrationContext(IAmbientServices ambientServices)
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
        /// Gets or sets the registrars.
        /// </summary>
        /// <value>
        /// The registrars.
        /// </value>
        public IEnumerable<IConventionsRegistrar> Registrars { get; set; }

        /// <summary>
        /// Gets or sets the application service information providers.
        /// </summary>
        /// <value>
        /// The application service information providers.
        /// </value>
        public IEnumerable<IAppServiceInfoProvider>? AppServiceInfoProviders { get; set; }
    }
}