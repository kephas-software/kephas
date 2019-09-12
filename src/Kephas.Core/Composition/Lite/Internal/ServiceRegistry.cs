﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service registry class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Lite.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Kephas.Resources;

    /// <summary>
    /// A service registry.
    /// </summary>
    internal class ServiceRegistry : IServiceRegistry
    {
        private readonly ConcurrentDictionary<Type, IServiceInfo> services =
            new ConcurrentDictionary<Type, IServiceInfo>();

        private readonly List<IServiceSource> serviceSources = new List<IServiceSource>();

        public IEnumerable<IServiceSource> Sources => this.serviceSources;

        public IServiceInfo this[Type contractType] => this.services[contractType];

        public bool TryGet(Type serviceType, out IServiceInfo serviceInfo) =>
            this.services.TryGetValue(serviceType, out serviceInfo);

        /// <summary>
        /// Gets a value indicating whether the service with the provided contract is registered.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>
        /// <c>true</c> if the service is registered, <c>false</c> if not.
        /// </returns>
        public bool IsRegistered(Type serviceType)
        {
            return serviceType != null && (this.services.ContainsKey(serviceType)
                                           || (serviceType.IsConstructedGenericType && this.services.ContainsKey(serviceType.GetGenericTypeDefinition()))
                                           || this.serviceSources.Any(s => s.IsMatch(serviceType)));
        }

        public ServiceRegistry RegisterService(IServiceInfo serviceInfo)
        {
            if (serviceInfo.AllowMultiple)
            {
                if (services.TryGetValue(serviceInfo.ContractType, out var existingServiceInfo))
                {
                    if (existingServiceInfo is MultiServiceInfo multiServiceInfo)
                    {
                        multiServiceInfo.Add((ServiceInfo)serviceInfo);
                        return this;
                    }

                    throw new InvalidOperationException(
                        string.Format(
                            Strings.ServiceRegistry_MismatchedMultipleServiceRegistration_Exception,
                            serviceInfo.ContractType));
                }

                serviceInfo = new MultiServiceInfo((ServiceInfo)serviceInfo);
            }
            else if (services.TryGetValue(serviceInfo.ContractType, out var existingServiceInfo)
                     && existingServiceInfo.AllowMultiple)
            {
                throw new InvalidOperationException(
                    string.Format(
                        Strings.ServiceRegistry_MismatchedMultipleServiceRegistration_Exception,
                        serviceInfo.ContractType));
            }

            services[serviceInfo.ContractType] = serviceInfo;
            return this;
        }

        public ServiceRegistry RegisterSource(IServiceSource serviceSource)
        {
            serviceSources.Add(serviceSource);
            return this;
        }

        public IEnumerator<IServiceInfo> GetEnumerator() => services.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Gets or registers a service.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceInfoGetter">The service info getter.</param>
        /// <returns>
        /// The existing or the registered service info.
        /// </returns>
        public IServiceInfo GetOrRegister(Type serviceType, Func<Type, IServiceInfo> serviceInfoGetter)
        {
            return this.services.GetOrAdd(serviceType, serviceInfoGetter);
        }
    }
}