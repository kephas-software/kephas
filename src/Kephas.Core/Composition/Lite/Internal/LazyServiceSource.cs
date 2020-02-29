// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LazyServiceSource.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the lazy service source class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Lite.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas;
    using Kephas.Reflection;
    using Kephas.Resources;

    internal class LazyServiceSource : ServiceSourceBase
    {
        private static readonly MethodInfo GetServiceMethod =
            ReflectionHelper.GetGenericMethodOf(_ => GetService<string>(null, null));

        public LazyServiceSource(IServiceRegistry registry)
            : base(registry)
        {
        }

        public override bool IsMatch(Type contractType)
        {
            return contractType.IsConstructedGenericOf(typeof(Lazy<>));
        }

        public override object GetService(IAmbientServices parent, Type serviceType)
        {
            var descriptors = this.GetServiceDescriptors(parent, serviceType);
            var (_, factory) = descriptors.SingleOrDefault();
            if (factory == null)
            {
                var innerType = serviceType.GetGenericArguments()[0];
                throw new CompositionException(Strings.NoImplementationForServiceType_Exception.FormatWith(innerType));
            }

            return factory();
        }

        public override IEnumerable<(IServiceInfo serviceInfo, Func<object> factory)> GetServiceDescriptors(
            IAmbientServices parent,
            Type serviceType)
        {
            var innerType = serviceType.GetGenericArguments()[0];
            var getService = GetServiceMethod.MakeGenericMethod(innerType);
            return this.GetServiceDescriptors(parent, innerType, ((IServiceInfo serviceInfo, Func<object> fn) tuple) => () => getService.Call(null, tuple.serviceInfo, tuple.fn));
        }

        private static Lazy<T> GetService<T>(IServiceInfo serviceInfo, Func<object> factory)
            where T : class
        {
            return new Lazy<T>(() => (T)factory());
        }
    }
}