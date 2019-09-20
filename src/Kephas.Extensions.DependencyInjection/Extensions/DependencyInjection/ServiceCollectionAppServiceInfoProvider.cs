// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionAppServiceInfoProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service collection conventions registrar class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Composition;
    using Kephas.Composition.Conventions;
    using Kephas.Composition.Hosting;
    using Kephas.Services;
    using Kephas.Services.Reflection;

    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A conventions registrar for a collection of service definitions.
    /// </summary>
    public class ServiceCollectionAppServiceInfoProvider : IAppServiceInfoProvider
    {
        /// <summary>
        /// Gets an enumeration of application service information objects.
        /// </summary>
        /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
        /// <param name="registrationContext">Context for the registration.</param>
        /// <returns>
        /// An enumeration of application service information objects and their associated contract type.
        /// </returns>
        public IEnumerable<(Type contractType, IAppServiceInfo appServiceInfo)> GetAppServiceInfos(
            IList<Type> candidateTypes,
            ICompositionRegistrationContext registrationContext)
        {
            var ambientServices = registrationContext.AmbientServices;
            var serviceCollection = ambientServices.GetService<IServiceCollection>();
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

                if (!candidateTypes.Contains(serviceType))
                {
                    candidateTypes.Add(serviceType);
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
                    if (!candidateTypes.Contains(instanceType))
                    {
                        candidateTypes.Add(instanceType);
                    }

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