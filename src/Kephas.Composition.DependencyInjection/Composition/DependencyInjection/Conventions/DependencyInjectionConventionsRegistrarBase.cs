// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionConventionsRegistrarBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the medi conventions registrar base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.DependencyInjection.Conventions
{
    using System;
    using System.Collections.Generic;

    using Kephas.Composition.Conventions;
    using Kephas.Composition.DependencyInjection.Resources;
    using Kephas.Composition.Hosting;
    using Kephas.Diagnostics.Contracts;

    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A Microsoft.Extensions.DependencyInjection conventions registrar base.
    /// </summary>
    public abstract class DependencyInjectionConventionsRegistrarBase : IConventionsRegistrar
    {
        /// <summary>
        /// Registers the conventions.
        /// </summary>
        /// <param name="builder">The registration builder.</param>
        /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
        /// <param name="registrationContext">Context for the registration.</param>
        public void RegisterConventions(
            IConventionsBuilder builder,
            IList<Type> candidateTypes,
            ICompositionRegistrationContext registrationContext)
        {
            Requires.NotNull(builder, nameof(builder));
            Requires.NotNull(candidateTypes, nameof(candidateTypes));
            Requires.NotNull(registrationContext, nameof(registrationContext));

            if (!(builder is IServiceCollectionProvider mediBuilder))
            {
                throw new InvalidOperationException(string.Format(Strings.InvalidConventions, typeof(IServiceCollectionProvider)));
            }

            this.RegisterConventions(mediBuilder.GetServiceCollection(), candidateTypes, registrationContext);
        }

        /// <summary>
        /// Registers the conventions.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
        /// <param name="registrationContext">Context for the registration.</param>
        protected abstract void RegisterConventions(
            IServiceCollection serviceCollection,
            IList<Type> candidateTypes,
            ICompositionRegistrationContext registrationContext);
    }
}