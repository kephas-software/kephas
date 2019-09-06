// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiServiceInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the multi service information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Lite.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Kephas.Composition;
    using Kephas.Services;
    using Kephas.Services.Reflection;

    internal class MultiServiceInfo : IServiceInfo, IEnumerable<IServiceInfo>
    {
        private IList<ServiceInfo> serviceInfos = new List<ServiceInfo>();

        public MultiServiceInfo(ServiceInfo serviceInfo)
        {
            ContractType = serviceInfo.ContractType;
            ServiceType = serviceInfo.ServiceType;
            serviceInfos.Add(serviceInfo);
        }

        AppServiceLifetime IAppServiceInfo.Lifetime => AppServiceLifetime.Transient;

        public bool AllowMultiple { get; } = true;

        bool IAppServiceInfo.AsOpenGeneric => false;

        Type[] IAppServiceInfo.MetadataAttributes => null;

        public Type ContractType { get; }

        object IAppServiceInfo.Instance => null;

        Type IAppServiceInfo.InstanceType => null;

        Func<ICompositionContext, object> IAppServiceInfo.InstanceFactory => null;

        public Type ServiceType { get; }

        public void Add(ServiceInfo serviceInfo)
        {
            serviceInfos.Add(serviceInfo);
        }

        public object GetService(IAmbientServices ambientServices)
        {
            throw new NotSupportedException("Only single service infos may provide services.");
        }

        public IDictionary<string, object> Metadata { get; }

        public IEnumerator<IServiceInfo> GetEnumerator() => serviceInfos.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}