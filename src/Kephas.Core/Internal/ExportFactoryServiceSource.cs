// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportFactoryServiceSource.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the export service source class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
    using Kephas.Reflection;

    internal class ExportFactoryServiceSource : ServiceSourceBase
    {
        private static readonly MethodInfo GetServiceMethod =
            ReflectionHelper.GetGenericMethodOf(_ => ExportFactoryServiceSource.GetService<string>(null, null));

        public ExportFactoryServiceSource(IServiceRegistry registry)
            : base(registry)
        {
        }

        public override bool IsMatch(Type contractType)
        {
            return contractType.IsConstructedGenericOf(typeof(IExportFactory<>));
        }

        public override object GetService(IAmbientServices parent, Type serviceType)
        {
            var descriptors = this.GetServiceDescriptors(parent, serviceType);
            return descriptors.Single().factory();
        }

        public override IEnumerable<(IServiceInfo serviceInfo, Func<object> factory)> GetServiceDescriptors(
            IAmbientServices parent,
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