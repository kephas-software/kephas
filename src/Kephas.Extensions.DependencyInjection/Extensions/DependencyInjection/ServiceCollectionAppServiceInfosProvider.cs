// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionAppServiceInfosProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service collection conventions registrar class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Extensions.DependencyInjection;
using Kephas.Services;

[assembly: AppServices(
    processingPriority: Priority.High - 100,
    providerType: typeof(ServiceCollectionAppServiceInfosProvider))]

namespace Kephas.Extensions.DependencyInjection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Injection;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Services.Reflection;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A conventions registrar for a collection of service definitions.
    /// </summary>
    public class ServiceCollectionAppServiceInfosProvider : IAppServiceInfosProvider
    {
        /// <summary>
        /// Gets an enumeration of application service information objects and their contract declaration type.
        /// The contract declaration type is the type declaring the contract: if the <see cref="AppServiceContractAttribute.ContractType"/>
        /// is not provided, the contract declaration type is also the contract type.
        /// </summary>
        /// <returns>
        /// An enumeration of application service information objects and their contract declaration type.
        /// </returns>
        public IEnumerable<ContractDeclaration> GetAppServiceContracts()
        {
            var ambientServices = context?.AmbientServices;
            var serviceCollection = ambientServices?.GetService<IServiceCollection>();
            if (serviceCollection == null)
            {
                yield break;
            }

            var openGenericServiceTypes = new HashSet<Type>(
                serviceCollection
                    .Where(s => s.ServiceType.ToNormalizedType().IsGenericTypeDefinition)
                    .Select(s => s.ServiceType.ToNormalizedType())
                    .ToList());

            // make sure to register first the open generics, so that the constructed generics
            // are later ignored
            foreach (var descriptor in serviceCollection)
            {
                var serviceType = descriptor.ServiceType.ToNormalizedType();
                var asOpenGeneric = serviceType.IsGenericTypeDefinition;
                if (serviceType.IsConstructedGenericType && openGenericServiceTypes.Contains(serviceType.GetGenericTypeDefinition()))
                {
                    continue;
                }

                // make sure AllowMultiple is set to true, as the ServiceCollection supports by default multiple registrations.
                if (descriptor.ImplementationInstance != null)
                {
                    yield return new ContractDeclaration(serviceType, new AppServiceInfo(serviceType, descriptor.ImplementationInstance) { AllowMultiple = true });
                }
                else if (descriptor.ImplementationFactory != null)
                {
                    yield return new ContractDeclaration(
                                        serviceType,
                                        new AppServiceInfo(
                                         serviceType,
                                         ctx => descriptor.ImplementationFactory(ctx.ToServiceProvider()))
                                         { AllowMultiple = true });
                }
                else
                {
                    var instanceType = descriptor.ImplementationType ?? serviceType;
                    var lifetime = descriptor.Lifetime == ServiceLifetime.Singleton
                                       ? AppServiceLifetime.Singleton
                                       : descriptor.Lifetime == ServiceLifetime.Scoped
                                           ? AppServiceLifetime.Scoped
                                           : AppServiceLifetime.Transient;

                    yield return new ContractDeclaration(
                                        serviceType,
                                        new AppServiceInfo(
                                         serviceType,
                                         instanceType,
                                         lifetime,
                                         asOpenGeneric)
                                         { AllowMultiple = true });
                }
            }
        }
    }
}