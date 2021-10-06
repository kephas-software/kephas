// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceSourceBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service source base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Lite.Internal
{
    using System;
    using System.Collections.Generic;

    using Kephas.Services;

    internal abstract class ServiceSourceBase : IServiceSource
    {
        protected readonly IAppServiceRegistry serviceRegistry;

        protected ServiceSourceBase(IAppServiceRegistry serviceRegistry)
        {
            this.serviceRegistry = serviceRegistry;
        }

        public abstract bool IsMatch(Type contractType);

        public abstract object GetService(IServiceProvider parent, Type serviceType);

        public virtual IEnumerable<(IServiceInfo serviceInfo, Func<object> factory)> GetServiceDescriptors(
            IServiceProvider parent,
            Type serviceType)
        {
            return this.GetServiceDescriptors(parent, serviceType, null);
        }

        protected virtual IEnumerable<(IServiceInfo serviceInfo, Func<object> factory)> GetServiceDescriptors(
            IServiceProvider serviceProvider,
            Type serviceType,
            Func<(IServiceInfo serviceInfo, Func<object> factory), Func<object>>? selector)
        {
            if (!this.serviceRegistry.TryGetSource(serviceType, out var serviceSource))
            {
                yield break;
            }

            switch (serviceSource)
            {
                case IEnumerable<IServiceInfo> multiServiceInfo:
                    foreach (var si in multiServiceInfo)
                    {
                        yield return
                            (si,
                                selector == null
                                    ? () => si.GetService(serviceProvider, si.ContractType!)
                                    : selector((si, () => si.GetService(serviceProvider, si.ContractType!))));
                    }

                    break;
                case IServiceInfo serviceInfo:
                    yield return (serviceInfo,
                        selector == null
                            ? () => serviceInfo.GetService(serviceProvider, serviceInfo.ContractType!)
                            : selector((serviceInfo, () => serviceInfo.GetService(serviceProvider, serviceInfo.ContractType!))));
                    break;
                case IServiceSource source:
                    foreach (var (serviceInfo, factory) in source.GetServiceDescriptors(serviceProvider, serviceType))
                    {
                        yield return (serviceInfo, selector == null ? factory : selector((serviceInfo, factory)));
                    }

                    break;
            }
        }
    }
}