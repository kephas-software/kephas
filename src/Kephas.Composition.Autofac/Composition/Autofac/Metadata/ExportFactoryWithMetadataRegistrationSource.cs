// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportFactoryWithMetadataRegistrationSource.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the export factory with metadata registration source class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Autofac.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using global::Autofac;
    using global::Autofac.Builder;
    using global::Autofac.Core;

    using Kephas.Composition.ExportFactories;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// An export factory with metadata registration source.
    /// </summary>
    public class ExportFactoryWithMetadataRegistrationSource : IRegistrationSource
    {
        private static readonly MethodInfo CreateMetaRegistrationMethod = ReflectionHelper.GetGenericMethodOf(
            _ => ((ExportFactoryWithMetadataRegistrationSource)null).CreateMetaRegistration<string, string>(null!, null!, null!));

        private readonly IRuntimeTypeRegistry typeRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportFactoryWithMetadataRegistrationSource"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        public ExportFactoryWithMetadataRegistrationSource(IRuntimeTypeRegistry typeRegistry)
        {
            this.typeRegistry = typeRegistry;
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
                || !swt.ServiceType.IsClosedTypeOf(typeof(IExportFactory<,>)))
            {
                return Enumerable.Empty<IComponentRegistration>();
            }

            var genericArgs = swt.ServiceType.GetTypeInfo().GenericTypeArguments.ToList();
            var valueType = genericArgs[0];
            var metadataType = genericArgs[1];

            var valueService = swt.ChangeType(valueType);

            var registrationCreator = CreateMetaRegistrationMethod.MakeGenericMethod(valueType, metadataType);

            return registrationAccessor(valueService)
                .Select(v => registrationCreator.Call(this, service, valueService, v))
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
            return "IExportFactory<T, TMetadata> support";
        }

        private IComponentRegistration CreateMetaRegistration<T, TMetadata>(Service providedService, Service valueService, IComponentRegistration valueRegistration)
        {
            var rb = RegistrationBuilder
                .ForDelegate((c, p) =>
                    {
                        var lifetimeScope = c.GetLifetimeScope();
                        return new ExportFactory<T, TMetadata>(
                            () =>
                            {
                                var request = new ResolveRequest(valueService, valueRegistration, p);
                                return (T)lifetimeScope.ResolveComponent(request);
                            },
                            (TMetadata)this.typeRegistry
                                .GetTypeInfo(typeof(TMetadata))
                                .CreateInstance(new object[] { valueRegistration.Target.Metadata }));
                    })
                .As(providedService)
                .Targeting(valueRegistration, isAdapterForIndividualComponent: false)
                .InheritRegistrationOrderFrom(valueRegistration);

            return rb.CreateRegistration();
        }
    }
}