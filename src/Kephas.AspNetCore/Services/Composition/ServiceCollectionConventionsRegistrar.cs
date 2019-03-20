// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionConventionsRegistrar.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service collection conventions registrar class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Services.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Composition;
    using Kephas.Composition.Conventions;
    using Kephas.Composition.Hosting;
    using Kephas.Services;
    using Kephas.Services.Reflection;

    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A conventions registrar for a collection of service definitions.
    /// </summary>
    public class ServiceCollectionConventionsRegistrar : IAppServiceInfoProvider
    {
        /// <summary>
        /// Registers the conventions.
        /// </summary>
        /// <param name="builder">The registration builder.</param>
        /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
        /// <param name="registrationContext">Context for the registration.</param>
        public void RegisterConventions(
            IConventionsBuilder builder,
            IEnumerable<TypeInfo> candidateTypes,
            ICompositionRegistrationContext registrationContext)
        {
            var ambientServices = registrationContext.AmbientServices;
            var serviceCollection = ambientServices.GetService<IServiceCollection>();

            foreach (var descriptor in serviceCollection)
            {
                if (descriptor.ImplementationInstance != null)
                {
                    builder.ForInstance(descriptor.ServiceType, descriptor.ImplementationInstance);
                }
                else if (descriptor.ImplementationFactory != null)
                {
                    builder.ForInstanceFactory(descriptor.ServiceType, ctx => descriptor.ImplementationFactory((ctx ?? ambientServices.CompositionContainer).ToServiceProvider()));
                }
                else
                {
                    var partBuilder = descriptor.ImplementationType != null
                                          ? builder.ForType(descriptor.ImplementationType)
                                          : builder.ForType(descriptor.ServiceType);

                    if (descriptor.Lifetime == ServiceLifetime.Singleton)
                    {
                        partBuilder.Shared();
                    }
                    else if (descriptor.Lifetime == ServiceLifetime.Scoped)
                    {
                        partBuilder.ScopeShared();
                    }

                    partBuilder.Export(b => b.AsContractType(descriptor.ServiceType));
                }
            }
        }

        /// <summary>
        /// Gets an enumeration of application service information objects.
        /// </summary>
        /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
        /// <param name="registrationContext">Context for the registration.</param>
        /// <returns>
        /// An enumeration of application service information objects and their associated contract type.
        /// </returns>
        public IEnumerable<(TypeInfo contractType, IAppServiceInfo appServiceInfo)> GetAppServiceInfos(IEnumerable<TypeInfo> candidateTypes, ICompositionRegistrationContext registrationContext)
        {
            var ambientServices = registrationContext.AmbientServices;
            var serviceCollection = ambientServices.GetService<IServiceCollection>();
            var openGenericServiceTypes = new List<Type>();

            foreach (var descriptor in serviceCollection.OrderBy(d => !d.ServiceType.IsGenericTypeDefinition))
            {
                var serviceType = descriptor.ServiceType;
                if (serviceType.IsGenericTypeDefinition)
                {
                    openGenericServiceTypes.Add(serviceType);
                }
                else if (serviceType.IsConstructedGenericType)
                {
                    if (openGenericServiceTypes.Contains(serviceType))
                    {
                        continue;
                    }
                }

                if (descriptor.ImplementationInstance != null)
                {
                    yield return (serviceType.GetTypeInfo(), new AppServiceInfo(serviceType, descriptor.ImplementationInstance));
                }
                else if (descriptor.ImplementationFactory != null)
                {
                    yield return (serviceType.GetTypeInfo(), new AppServiceInfo(serviceType, descriptor.ImplementationFactory));
                }
                else
                {
                    var instanceType = descriptor.ImplementationType ?? serviceType;
                    var lifetime = descriptor.Lifetime == ServiceLifetime.Singleton
                                       ? AppServiceLifetime.Shared
                                       : descriptor.Lifetime == ServiceLifetime.Scoped
                                           ? AppServiceLifetime.ScopeShared
                                           : AppServiceLifetime.Instance;

                    yield return (serviceType.GetTypeInfo(), new AppServiceInfo(serviceType, instanceType, lifetime));
                }
            }
        }
    }
}