// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemCompositionConventionsRegistrarBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base class for MEF conventions registrars.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.SystemComposition.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Convention;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection.Conventions;
    using Kephas.Injection.Hosting;
    using Kephas.Injection.SystemComposition.Resources;

    /// <summary>
    /// Base class for MEF conventions registrars.
    /// </summary>
    public abstract class SystemCompositionConventionsRegistrarBase : IConventionsRegistrar
    {
        /// <summary>
        /// Registers the conventions.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the provided conventions are not MEF conventions.</exception>
        /// <param name="builder">The registration builder.</param>
        /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
        /// <param name="buildContext">Context for the registration.</param>
        public void RegisterConventions(
            IConventionsBuilder builder,
            IList<Type> candidateTypes,
            IInjectionBuildContext buildContext)
        {
            Requires.NotNull(builder, nameof(builder));
            Requires.NotNull(candidateTypes, nameof(candidateTypes));
            Requires.NotNull(buildContext, nameof(buildContext));

            if (!(builder is ISystemCompositionConventionBuilderProvider mefBuilder))
            {
                throw new InvalidOperationException(string.Format(Strings.InvalidConventions, typeof(ISystemCompositionConventionBuilderProvider)));
            }

            this.RegisterConventions(mefBuilder.GetConventionBuilder(), candidateTypes, buildContext);
        }

        /// <summary>
        /// Registers the conventions.
        /// </summary>
        /// <param name="builder">The registration builder.</param>
        /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
        /// <param name="buildContext">Context for the registration.</param>
        protected abstract void RegisterConventions(
            ConventionBuilder builder,
            IList<Type> candidateTypes,
            IInjectionBuildContext buildContext);
    }
}