// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefConventionsRegistrarBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base class for MEF conventions registrars.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Convention;
    using System.Reflection;

    using Kephas.Composition.Conventions;
    using Kephas.Composition.Hosting;
    using Kephas.Composition.Mef.Resources;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Base class for MEF conventions registrars.
    /// </summary>
    public abstract class MefConventionsRegistrarBase : IConventionsRegistrar
    {
        /// <summary>
        /// Registers the conventions.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the provided conventions are not MEF conventions.</exception>
        /// <param name="builder">The registration builder.</param>
        /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
        /// <param name="registrationContext">Context for the registration.</param>
        public void RegisterConventions(IConventionsBuilder builder, IEnumerable<TypeInfo> candidateTypes, ICompositionRegistrationContext registrationContext)
        {
            Requires.NotNull(builder, nameof(builder));
            Requires.NotNull(candidateTypes, nameof(candidateTypes));
            Requires.NotNull(registrationContext, nameof(registrationContext));

            var mefBuilder = builder as IMefConventionBuilderProvider;
            if (mefBuilder == null)
            {
                throw new InvalidOperationException(string.Format(Strings.InvalidConventions, typeof(IMefConventionBuilderProvider)));
            }

            this.RegisterConventions(mefBuilder.GetConventionBuilder(), candidateTypes, registrationContext);
        }

        /// <summary>
        /// Registers the conventions.
        /// </summary>
        /// <param name="builder">The registration builder.</param>
        /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
        /// <param name="registrationContext">Context for the registration.</param>
        protected abstract void RegisterConventions(ConventionBuilder builder, IEnumerable<TypeInfo> candidateTypes, ICompositionRegistrationContext registrationContext);
    }
}