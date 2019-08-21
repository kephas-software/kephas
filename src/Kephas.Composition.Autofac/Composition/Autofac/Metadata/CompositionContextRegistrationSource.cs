// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionContextRegistrationSource.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the composition context registration source class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Autofac.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Autofac;
    using global::Autofac.Builder;
    using global::Autofac.Core;

    using Kephas.Composition.Autofac.Hosting;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A composition context registration source.
    /// </summary>
    internal class CompositionContextRegistrationSource : IRegistrationSource
    {
        private readonly ICompositionContainer compositionContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionContextRegistrationSource"/>
        /// class.
        /// </summary>
        /// <param name="compositionContainer">Context for the root composition.</param>
        public CompositionContextRegistrationSource(ICompositionContainer compositionContainer)
        {
            this.compositionContainer = compositionContainer;
        }

        /// <summary>
        /// Gets a value indicating whether the registrations provided by this source are 1:1 adapters on
        /// top of other components (e.g., Meta, Func, or Owned).
        /// </summary>
        /// <value>
        /// True if this object is adapter for individual components, false if not.
        /// </value>
        public bool IsAdapterForIndividualComponents => true;

        /// <summary>
        /// Retrieve registrations for an unregistered service, to be used by the container.
        /// </summary>
        /// <param name="service">The service that was requested.</param>
        /// <param name="registrationAccessor">A function that will return existing registrations for a
        ///                                    service.</param>
        /// <returns>
        /// Registrations providing the service.
        /// </returns>
        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
        {
            Requires.NotNull(registrationAccessor, nameof(registrationAccessor));

            if (!(service is IServiceWithType swt)
                || !ReferenceEquals(swt.ServiceType, typeof(ICompositionContext)))
            {
                return Enumerable.Empty<IComponentRegistration>();
            }

            return registrationAccessor(service)
                .Select(v => this.CreateMetaRegistration(service, service, v));
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return "ICompositionContext<T> support";
        }

        private IComponentRegistration CreateMetaRegistration(Service providedService, Service valueService, IComponentRegistration valueRegistration)
        {
            var rb = RegistrationBuilder
                .ForDelegate<ICompositionContext>((c, p) => this.compositionContainer.TryGetCompositionContext(c, createNewIfMissing: true))
                .As(providedService)
                .Targeting(valueRegistration)
                .InheritRegistrationOrderFrom(valueRegistration);

            return rb.CreateRegistration();
        }
    }
}