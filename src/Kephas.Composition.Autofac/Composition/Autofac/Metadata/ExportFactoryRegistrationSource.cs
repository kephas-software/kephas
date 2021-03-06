﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportFactoryRegistrationSource.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the export factory registration source class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Autofac.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using global::Autofac;
    using global::Autofac.Builder;
    using global::Autofac.Core;

    using Kephas.Composition.ExportFactories;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;

    /// <summary>
    /// An export factory registration source.
    /// </summary>
    public class ExportFactoryRegistrationSource : IRegistrationSource
    {
        private static readonly MethodInfo CreateMetaRegistrationMethod = ReflectionHelper.GetGenericMethodOf(
            _ => ExportFactoryRegistrationSource.CreateMetaRegistration<string>(null!, null!, default));

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
        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<ServiceRegistration>> registrationAccessor)
        {
            Requires.NotNull(registrationAccessor, nameof(registrationAccessor));

            if (!(service is IServiceWithType swt)
                || swt.ServiceType.IsClosedTypeOf(typeof(IExportFactory<,>))
                || !swt.ServiceType.IsClosedTypeOf(typeof(IExportFactory<>)))
            {
                return Enumerable.Empty<IComponentRegistration>();
            }

            var valueType = swt.ServiceType.GetTypeInfo().GenericTypeArguments.First();

            var valueService = swt.ChangeType(valueType);

            var registrationCreator = CreateMetaRegistrationMethod.MakeGenericMethod(valueType);

            return registrationAccessor(valueService)
                .Select(v => registrationCreator.Call(null, service, valueService, v))
                .Cast<IComponentRegistration>();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return "IExportFactory<T> support";
        }

        private static IComponentRegistration CreateMetaRegistration<T>(Service providedService, Service valueService, ServiceRegistration valueRegistration)
        {
            var rb = RegistrationBuilder
                .ForDelegate((c, p) =>
                    {
                        var lifetimeScope = c.GetLifetimeScope();
                        var request = new ResolveRequest(valueService, valueRegistration, p);
                        return new ExportFactory<T>(() => (T)lifetimeScope.ResolveComponent(request));
                    })
                .As(providedService)
                .Targeting(valueRegistration.Registration)
                .InheritRegistrationOrderFrom(valueRegistration.Registration);

            return rb.CreateRegistration();
        }
    }
}