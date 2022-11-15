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

    /// <summary>
    /// Service information for multiple services.
    /// </summary>
    internal class MultiServiceInfo : IServiceInfo, IEnumerable<IServiceInfo>, IDisposable
    {
        private readonly IList<ServiceInfo> serviceInfos = new List<ServiceInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiServiceInfo"/> class.
        /// </summary>
        /// <param name="serviceInfo">Information describing the service.</param>
        public MultiServiceInfo(ServiceInfo serviceInfo)
        {
            this.ContractType = serviceInfo.ContractType;
            this.serviceInfos.Add(serviceInfo);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiServiceInfo"/> class.
        /// This registration is for the case of multiple services without any implementation.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        internal MultiServiceInfo(Type contractType)
            : this(contractType, Array.Empty<ServiceInfo>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiServiceInfo"/> class.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="serviceInfos">The service infos.</param>
        private MultiServiceInfo(Type contractType, IEnumerable<ServiceInfo> serviceInfos)
        {
            this.ContractType = contractType;
            this.serviceInfos.AddRange(serviceInfos);
        }

        AppServiceLifetime IAppServiceInfo.Lifetime => AppServiceLifetime.Transient;

        public bool AllowMultiple { get; } = true;

        bool IAppServiceInfo.AsOpenGeneric => false;

        public Type? ContractType { get; }

        public object? InstancingStrategy => null;

        public void Add(ServiceInfo serviceInfo)
        {
            this.serviceInfos.Add(serviceInfo);
        }

        public object GetService(IServiceProvider serviceProvider, Type contractType)
        {
            if (this.serviceInfos.Count == 0)
            {
                throw new InjectionException($"No service resolving strategy registered for ({contractType}).");
            }

            // resolve the service from the last registration.
            return this.serviceInfos[^1].GetService(serviceProvider, contractType);
        }

        public IDictionary<string, object?>? Metadata { get; }

        IAppServiceInfo IAppServiceInfo.AddMetadata(string name, object? value) =>
            throw new NotSupportedException($"Only single service infos may add metadata ({this.ContractType}).");

        public IEnumerator<IServiceInfo> GetEnumerator() => serviceInfos.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IServiceInfo MakeGenericServiceInfo(Type[] genericArgs)
        {
            if (!this.ContractType.IsGenericTypeDefinition)
            {
                throw new NotSupportedException($"Only open generic registrations may be constructed, {this} does not support this operation.");
            }

            var closedContractType = this.ContractType.MakeGenericType(genericArgs);

            var closedServiceInfos = this.serviceInfos.Select(si => (ServiceInfo)si.MakeGenericServiceInfo(genericArgs));

            return new MultiServiceInfo(closedContractType, closedServiceInfos);
        }

        public void Dispose()
        {
            this.serviceInfos.ForEach(svc => (svc as IDisposable)?.Dispose());
            this.serviceInfos.Clear();
        }

        public bool IsMatch(Type contractType) => contractType == this.ContractType;
    }
}