// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICompositionRegistrationContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    }
}