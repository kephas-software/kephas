﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionRegistrationContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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