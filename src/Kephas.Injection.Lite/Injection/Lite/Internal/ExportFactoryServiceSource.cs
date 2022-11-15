// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportFactoryServiceSource.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the export service source class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Lite.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Reflection;
    using Kephas.Resources;
    using Kephas.Runtime;
    using Kephas.Services;

    internal class ExportFactoryServiceSource : GenericServiceSourceBase
    {
        private static readonly MethodInfo GetServiceMethod =
            ReflectionHelper.GetGenericMethodOf(_ => GetService<string>(null, null));

        public ExportFactoryServiceSource(IAppServiceRegistry serviceRegistry)
            : base(serviceRegistry, typeof(IExportFactory<>))
        {
        }

        public override object GetService(IServiceProvider parent, Type serviceType)
        {
            var descriptors = this.GetServiceDescriptors(parent, serviceType);
            var (_, factory) = descriptors.SingleOrDefault();
            if (factory == null)
            {
                var innerType = serviceType.GetGenericArguments()[0];
                throw new InjectionException(AbstractionStrings.NoImplementationForServiceType_Exception.FormatWith(innerType));
            }

            return factory();
        }

        public override IEnumerable<(IServiceInfo serviceInfo, Func<object> factory)> GetServiceDescriptors(
            IServiceProvider parent,
            Type serviceType)
        {
            var innerType = serviceType.GetGenericArguments()[0];
            var getService = GetServiceMethod.MakeGenericMethod(innerType);
            return this.GetServiceDescriptors(parent, innerType, ((IServiceInfo serviceInfo, Func<object> fn) tuple) => () => getService.Call(null, tuple.serviceInfo, tuple.fn));
        }

        private static IExportFactory<T> GetService<T>(IServiceInfo serviceInfo, Func<object> factory)
            where T : class
        {
            return new ExportFactory<T>(() => (T)factory());
        }
    }
}