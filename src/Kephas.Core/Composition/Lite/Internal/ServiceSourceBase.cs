// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceSourceBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service source base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Lite.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal abstract class ServiceSourceBase : IServiceSource
    {
        protected readonly IServiceRegistry registry;

        protected ServiceSourceBase(IServiceRegistry registry)
        {
            this.registry = registry;
        }

        public abstract bool IsMatch(Type contractType);

        public abstract object GetService(IAmbientServices parent, Type serviceType);

        public virtual IEnumerable<(IServiceInfo serviceInfo, Func<object> factory)> GetServiceDescriptors(
            IAmbientServices parent,
            Type serviceType)
        {
            return GetServiceDescriptors(parent, serviceType, null);
        }

        protected virtual IEnumerable<(IServiceInfo serviceInfo, Func<object> factory)> GetServiceDescriptors(
            IAmbientServices parent,
            Type serviceType,
            Func<(IServiceInfo serviceInfo, Func<object> factory), Func<object>> selector)
        {
            if (this.registry.TryGet(serviceType, out var serviceInfo))
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
                var source = registry.Sources.FirstOrDefault(s => s.IsMatch(serviceType));
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