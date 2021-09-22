// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiServiceInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the multi service information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Lite.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Services;
    using Kephas.Services.Reflection;

    internal class MultiServiceInfo : IServiceInfo, IEnumerable<IServiceInfo>, IDisposable
    {
        private IList<ServiceInfo> serviceInfos = new List<ServiceInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiServiceInfo"/> class.
        /// </summary>
        /// <param name="serviceInfo">Information describing the service.</param>
        public MultiServiceInfo(ServiceInfo serviceInfo)
        {
            this.ContractType = serviceInfo.ContractType;
            this.ServiceType = serviceInfo.ServiceType;
            this.serviceInfos.Add(serviceInfo);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiServiceInfo"/> class.
        /// This registration is for the case of multiple services without any implementation.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="serviceType">Type of the service.</param>
        internal MultiServiceInfo(Type contractType, Type serviceType)
            : this(contractType, serviceType, Array.Empty<ServiceInfo>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiServiceInfo"/> class.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceInfos">The service infos.</param>
        private MultiServiceInfo(Type contractType, Type serviceType, IEnumerable<ServiceInfo> serviceInfos)
        {
            this.ContractType = contractType;
            this.ServiceType = serviceType;
            this.serviceInfos.AddRange(serviceInfos);
        }

        AppServiceLifetime IAppServiceInfo.Lifetime => AppServiceLifetime.Transient;

        public bool AllowMultiple { get; } = true;

        bool IAppServiceInfo.AsOpenGeneric => false;

        Type[]? IAppServiceInfo.MetadataAttributes => null;

        public Type? ContractType { get; }

        public object? InstancingStrategy => null;

        public Type ServiceType { get; }

        public void Add(ServiceInfo serviceInfo)
        {
            this.serviceInfos.Add(serviceInfo);
        }

        public object GetService(IAmbientServices ambientServices)
        {
            throw new NotSupportedException("Only single service infos may provide services.");
        }

        public IDictionary<string, object?>? Metadata { get; }

        public IEnumerator<IServiceInfo> GetEnumerator() => serviceInfos.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IServiceInfo MakeGenericServiceInfo(IAmbientServices ambientServices, Type[] genericArgs)
        {
            if (!this.ContractType.IsGenericTypeDefinition)
            {
                throw new NotSupportedException($"Only open generic registrations may be constructed, {this} does not support this operation.");
            }

            var closedContractType = this.ContractType.MakeGenericType(genericArgs);
            var closedServiceType = this.ServiceType?.MakeGenericType(genericArgs);

            var closedServiceInfos = this.serviceInfos.Select(si => (ServiceInfo)si.MakeGenericServiceInfo(ambientServices, genericArgs));

            return new MultiServiceInfo(closedContractType, closedServiceType, closedServiceInfos);
        }

        public void Dispose()
        {
            this.serviceInfos.ForEach(svc => (svc as IDisposable)?.Dispose());
            this.serviceInfos.Clear();
        }
    }
}