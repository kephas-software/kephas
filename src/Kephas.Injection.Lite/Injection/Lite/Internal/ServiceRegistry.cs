// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service serviceRegistry class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Lite.Internal
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Resources;
    using Kephas.Services;
    using Kephas.Services.Reflection;

    /// <summary>
    /// A service serviceRegistry.
    /// </summary>
    internal class ServiceRegistry : IAppServiceRegistry
    {
        private readonly ConcurrentDictionary<Type, IAppServiceInfo> services = new ();

        private readonly List<IAppServiceSource> serviceSources = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRegistry"/> class.
        /// </summary>
        public ServiceRegistry()
        {
            this
                .RegisterSource(new LazyServiceSource(this))
                .RegisterSource(new LazyWithMetadataServiceSource(this))
                .RegisterSource(new ExportFactoryServiceSource(this))
                .RegisterSource(new ExportFactoryWithMetadataServiceSource(this))
                .RegisterSource(new ListServiceSource(this))
                .RegisterSource(new CollectionServiceSource(this))
                .RegisterSource(new EnumerableServiceSource(this));
        }

        /// <summary>
        /// Attempts to get the <see cref="IAppServiceSource"/> for the given service contract.
        /// </summary>
        /// <param name="contractType">Type of the service contract.</param>
        /// <param name="appServiceSource">The <see cref="IAppServiceSource"/> instance.</param>
        /// <returns>
        /// True if a <see cref="IAppServiceSource"/> is found, otherwise false.
        /// </returns>
        public bool TryGetSource(Type contractType, out IAppServiceSource? appServiceSource)
        {
            // simple registration
            if (this.services.TryGetValue(contractType, out var serviceRegistration) && serviceRegistration is IAppServiceSource serviceSource)
            {
                appServiceSource = serviceSource;
                return true;
            }

            // open generic registration
            if (contractType.IsConstructedGenericType)
            {
                var openServiceType = contractType.GetGenericTypeDefinition();

                if (this.services.TryGetValue(openServiceType, out serviceRegistration) && serviceRegistration is IServiceInfo genericServiceInfo)
                {
                    serviceRegistration = this.services.GetOrAdd(contractType, _ => genericServiceInfo.MakeGenericServiceInfo(contractType.GetGenericArguments()));
                    if (serviceRegistration is IAppServiceSource genericServiceSource)
                    {
                        appServiceSource = genericServiceSource;
                        return true;
                    }
                }
            }

            // source registration
            appServiceSource = this.serviceSources.FirstOrDefault(source => source.IsMatch(contractType));
            return appServiceSource != null;
        }

        /// <summary>
        /// Registers the source described by <paramref name="serviceSource"/>.
        /// </summary>
        /// <param name="serviceSource">The service source.</param>
        /// <returns>
        /// This service registry.
        /// </returns>
        public IAppServiceRegistry RegisterSource(IAppServiceSource serviceSource)
        {
            if (serviceSource is IAppServiceInfo serviceInfo)
            {
                return this.Register(serviceInfo);
            }

            this.serviceSources.Add(serviceSource);
            return this;
        }

        /// <summary>
        /// Gets an enumeration of application service information objects and their contract declaration type.
        /// The contract declaration type is the type declaring the contract: if the <see cref="AppServiceContractAttribute.ContractType"/>
        /// is not provided, the contract declaration type is also the contract type.
        /// </summary>
        /// <param name="context">Optional. The context in which the service types are requested.</param>
        /// <returns>
        /// An enumeration of application service information objects and their contract declaration type.
        /// </returns>
        public IEnumerable<ContractDeclaration> GetAppServiceContracts(IContext? context = null) =>
            this.services.Values
                .SelectMany(s => this.ToAppServiceInfos(s).Select(si => new ContractDeclaration(si.ContractType!, si)));

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            this.services.ForEach(kv => (kv.Value as IDisposable)?.Dispose());
            this.services.Clear();
            this.serviceSources.Clear();
        }

        /// <summary>
        /// Registers the service described by <paramref name="serviceInfo"/>.
        /// </summary>
        /// <param name="serviceInfo">Information describing the service.</param>
        /// <returns>
        /// This service registry.
        /// </returns>
        private IAppServiceRegistry Register(IAppServiceInfo serviceInfo)
        {
            var contractType = serviceInfo.ContractType ?? throw new ArgumentException($"The service information does not have the '{nameof(IServiceInfo.ContractType)}' value set.", nameof(serviceInfo));
            if (serviceInfo.AllowMultiple)
            {
                if (this.services.TryGetValue(contractType, out var existingServiceInfo))
                {
                    if (existingServiceInfo is not MultiServiceInfo multiServiceInfo)
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                AbstractionStrings.ServiceRegistry_MismatchedMultipleServiceRegistration_Exception,
                                contractType));
                    }

                    multiServiceInfo.Add((ServiceInfo)serviceInfo);
                    return this;
                }

                serviceInfo = serviceInfo as MultiServiceInfo ?? new MultiServiceInfo((ServiceInfo)serviceInfo);
            }
            else if (this.services.TryGetValue(contractType, out var existingServiceInfo)
                     && existingServiceInfo.AllowMultiple)
            {
                throw new InvalidOperationException(
                    string.Format(
                        AbstractionStrings.ServiceRegistry_MismatchedMultipleServiceRegistration_Exception,
                        contractType));
            }

            this.services[contractType] = serviceInfo;
            return this;
        }

        private IEnumerable<IAppServiceInfo> ToAppServiceInfos(IAppServiceInfo appServiceInfo)
        {
            switch (appServiceInfo)
            {
                case IEnumerable<IServiceInfo> multiServiceInfos:
                    foreach (ServiceInfo si in multiServiceInfos)
                    {
                        yield return si.ToAppServiceInfo();
                    }

                    break;
                case ServiceInfo serviceInfo:
                    yield return serviceInfo.ToAppServiceInfo();
                    break;
                default:
                    yield return appServiceInfo;
                    break;
            }
        }
    }
}