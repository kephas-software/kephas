// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionAppServiceInfoProvider.cs" company="Kephas Software SRL">
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
    serviceProviderTypes: new[] { typeof(ServiceCollectionAppServiceInfoProvider) })]

namespace Kephas.Extensions.DependencyInjection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Injection;
    using Kephas.Injection.Hosting;
    using Kephas.Services;
    using Kephas.Services.Reflection;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A conventions registrar for a collection of service definitions.
    /// </summary>
    public class ServiceCollectionAppServiceInfoProvider : IAppServiceInfoProvider
    {
        /// <summary>
        /// Gets an enumeration of application service information objects and their contract declaration type.
        /// The contract declaration type is the type declaring the contract: if the <see cref="AppServiceContractAttribute.ContractType"/>
        /// is not provided, the contract declaration type is also the contract type.
        /// </summary>
        /// <param name="context">Optional. The context in which the service types are requested.</param>
        /// <returns>
        /// An enumeration of application service information objects and their contract declaration type.
        /// </returns>
        public IEnumerable<(Type contractDeclarationType, IAppServiceInfo appServiceInfo)> GetAppServiceInfos(dynamic? context = null)
        {
            var ambientServices = ((IInjectionRegistrationContext?)context)?.AmbientServices;
            if (ambientServices == null)
            {
                yield break;
            }

            var serviceCollection = ambientServices.GetRequiredService<IServiceCollection>();
            var openGenericServiceTypes = new List<Type>();

            // make sure to register first the open generics, so that the constructed generics
            // are later ignored
            foreach (var descriptor in serviceCollection.OrderBy(d => d.ServiceType.IsGenericTypeDefinition ? 0 : 1))
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
                    yield return (serviceType, new AppServiceInfo(serviceType, descriptor.ImplementationInstance));
                }
                else if (descriptor.ImplementationFactory != null)
                {
                    yield return (serviceType,
                                     new AppServiceInfo(
                                         serviceType,
                                         ctx => descriptor.ImplementationFactory(ctx.ToServiceProvider())));
                }
                else
                {
                    var instanceType = descriptor.ImplementationType ?? serviceType;
                    var lifetime = descriptor.Lifetime == ServiceLifetime.Singleton
                                       ? AppServiceLifetime.Singleton
                                       : descriptor.Lifetime == ServiceLifetime.Scoped
                                           ? AppServiceLifetime.Scoped
                                           : AppServiceLifetime.Transient;

                    yield return (serviceType,
                                     new AppServiceInfo(
                                         serviceType,
                                         instanceType,
                                         lifetime,
                                         serviceType.IsGenericTypeDefinition));
                }
            }
        }
    }
}