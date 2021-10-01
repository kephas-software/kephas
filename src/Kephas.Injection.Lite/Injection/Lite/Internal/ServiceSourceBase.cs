// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceSourceBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service source base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Kephas.Runtime;

namespace Kephas.Injection.Lite.Internal
{
    internal abstract class ServiceSourceBase : IServiceSource
    {
        protected readonly IServiceRegistry serviceRegistry;
        protected readonly IRuntimeTypeRegistry typeRegistry;

        protected ServiceSourceBase(IServiceRegistry serviceRegistry, IRuntimeTypeRegistry typeRegistry)
        {
            this.serviceRegistry = serviceRegistry;
            this.typeRegistry = typeRegistry;
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
            IServiceProvider parent,
            Type serviceType,
            Func<(IServiceInfo serviceInfo, Func<object> factory), Func<object>>? selector)
        {
            if (this.serviceRegistry.TryGet(serviceType, out var serviceInfo))
            {
                if (serviceInfo is IEnumerable<IServiceInfo> multiServiceInfo)
                {
                    foreach (var si in multiServiceInfo)
                    {
                        yield return
                            (si,
                                selector == null
                                    ? () => si.GetService(parent)
                                    : selector((si, () => si.GetService(parent))));
                    }
                }
                else
                {
                    yield return (serviceInfo,
                                     selector == null
                                         ? () => serviceInfo.GetService(parent)
                                         : selector((serviceInfo, () => serviceInfo.GetService(parent))));
                }
            }
            else
            {
                var source = this.serviceRegistry.Sources.FirstOrDefault(s => s.IsMatch(serviceType));
                if (source != null)
                {
                    foreach (var descriptor in source.GetServiceDescriptors(parent, serviceType))
                    {
                        yield return (descriptor.serviceInfo,
                                         selector == null ? descriptor.factory : selector((descriptor.serviceInfo, descriptor.factory)));
                    }
                }
            }
        }
    }
}