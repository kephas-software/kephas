// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICompositionRegistrationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ICompositionRegistrationContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Hosting
{
    using System;
    using System.Collections.Generic;

    using Kephas.Composition.Conventions;
    using Kephas.Services;

    /// <summary>
    /// Contract interface for composition container builder contexts.
    /// </summary>
    public interface ICompositionRegistrationContext : IContext
    {
        /// <summary>
        /// Gets or sets the parts.
        /// </summary>
        /// <value>
        /// The parts.
        /// </value>
        IEnumerable<Type> Parts { get; set; }

        /// <summary>
        /// Gets or sets the registrars.
        /// </summary>
        /// <value>
        /// The registrars.
        /// </value>
        IEnumerable<IConventionsRegistrar> Registrars { get; set; }

        /// <summary>
        /// Gets or sets the application service information providers.
        /// </summary>
        /// <value>
        /// The application service information providers.
        /// </value>
        IEnumerable<IAppServiceInfoProvider> AppServiceInfoProviders { get; set; }
    }
}