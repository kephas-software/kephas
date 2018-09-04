// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionRegistrationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the composition container builder context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Hosting
{
    using System;
    using System.Collections.Generic;

    using Kephas.Composition.Conventions;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// A composition container builder context.
    /// </summary>
    public class CompositionRegistrationContext : Context, ICompositionRegistrationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionRegistrationContext"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        public CompositionRegistrationContext(IAmbientServices ambientServices)
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
        public IEnumerable<Type> Parts { get; set; }

        /// <summary>
        /// Gets or sets the registrars.
        /// </summary>
        /// <value>
        /// The registrars.
        /// </value>
        public IEnumerable<IConventionsRegistrar> Registrars { get; set; }
    }
}