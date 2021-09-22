// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacConventionsRegistrarBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the Autofac conventions registrar base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Autofac.Conventions
{
    using System;
    using System.Collections.Generic;
    using global::Autofac;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection.Autofac.Resources;
    using Kephas.Injection.Conventions;
    using Kephas.Injection.Hosting;

    /// <summary>
    /// An Autofac conventions registrar base.
    /// </summary>
    public abstract class AutofacConventionsRegistrarBase : IConventionsRegistrar
    {
        /// <summary>
        /// Registers the conventions.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="builder">The registration builder.</param>
        /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
        /// <param name="registrationContext">Context for the registration.</param>
        public void RegisterConventions(
            IConventionsBuilder builder,
            IList<Type> candidateTypes,
            IInjectionRegistrationContext registrationContext)
        {
            Requires.NotNull(builder, nameof(builder));
            Requires.NotNull(candidateTypes, nameof(candidateTypes));
            Requires.NotNull(registrationContext, nameof(registrationContext));

            if (!(builder is IAutofacContainerBuilderProvider autofacBuilder))
            {
                throw new InvalidOperationException(string.Format(Strings.InvalidConventions, typeof(IAutofacContainerBuilderProvider)));
            }

            this.RegisterConventions(autofacBuilder.GetContainerBuilder(), candidateTypes, registrationContext);
        }

        /// <summary>
        /// Registers the conventions.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
        /// <param name="registrationContext">Context for the registration.</param>
        protected abstract void RegisterConventions(
            ContainerBuilder builder,
            IList<Type> candidateTypes,
            IInjectionRegistrationContext registrationContext);
    }
}