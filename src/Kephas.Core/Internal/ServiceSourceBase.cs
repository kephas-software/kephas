// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceSourceBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service source base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Internal
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

        public abstract object GetService(IServiceProvider parent, Type serviceType);

        public virtual IEnumerable<(IServiceInfo serviceInfo, Func<object> factory)> GetServiceDescriptors(IServiceProvider parent, Type serviceType)
        {
            return this.GetServiceDescriptors(parent, serviceType, null);
        }

        protected virtual IEnumerable<(IServiceInfo serviceInfo, Func<object> factory)> GetServiceDescriptors(IServiceProvider parent, Type serviceType, Func<Func<object>, Func<object>> selector)
        {
            if (this.registry.TryGetValue(serviceType, out var serviceInfo))
            {
                if (serviceInfo is IEnumerable<IServiceInfo> multiServiceInfo)
                {
                    foreach (var si in multiServiceInfo)
                    {
                        yield return
                            (si,
                                selector == null
                                    ? () => si.GetService((AmbientServices)parent)
                                    : selector(() => si.GetService((AmbientServices)parent)));
                    }
                }
                else
                {
                    yield return (serviceInfo,
                                     selector == null
                                         ? () => serviceInfo.GetService((AmbientServices)parent)
                                         : selector(() => serviceInfo.GetService((AmbientServices)parent)));
                }
            }
            else
            {
                var source = this.registry.Sources.FirstOrDefault(s => s.IsMatch(serviceType));
                if (source != null)
                {
                    foreach (var descriptor in source.GetServiceDescriptors(parent, serviceType))
                    {
                        yield return (descriptor.serviceInfo,
                                         selector == null ? descriptor.factory : selector(descriptor.factory));
                    }
                }
            }
        }
    }
}