// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service registry class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    internal class ServiceRegistry : IServiceRegistry
    {
        private readonly ConcurrentDictionary<Type, IServiceInfo> services =
            new ConcurrentDictionary<Type, IServiceInfo>();

        private readonly List<IServiceSource> serviceSources = new List<IServiceSource>();

        public IServiceInfo this[Type contractType] => this.services[contractType];

        public IEnumerable<IServiceSource> Sources => this.serviceSources;

        public bool TryGetValue(Type serviceType, out IServiceInfo serviceInfo) =>
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
            return serviceType != null && (this.services.ContainsKey(serviceType) || this.serviceSources.Any(s => s.IsMatch(serviceType)));
        }

        public ServiceRegistry RegisterService(IServiceInfo serviceInfo)
        {
            this.services[serviceInfo.ContractType] = serviceInfo;
            return this;
        }

        public ServiceRegistry RegisterSource(IServiceSource serviceSource)
        {
            this.serviceSources.Add(serviceSource);
            return this;
        }

        public IEnumerator<IServiceInfo> GetEnumerator() => this.services.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}