// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LazyWithMetadataServiceSource.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the lazy with metadata service source class.
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

    internal class LazyWithMetadataServiceSource : ServiceSourceBase
    {
        private static readonly MethodInfo GetServiceMethod =
            ReflectionHelper.GetGenericMethodOf(_ => LazyWithMetadataServiceSource.GetService<string, string>(null, null, null, null));

        private readonly IAppServiceMetadataResolver metadataResolver;

        public LazyWithMetadataServiceSource(IServiceRegistry serviceRegistry, IRuntimeTypeRegistry typeRegistry)
            : base(serviceRegistry, typeRegistry)
        {
            this.metadataResolver = new AppServiceMetadataResolver(typeRegistry);
        }

        public override bool IsMatch(Type contractType)
        {
            return contractType.IsConstructedGenericOf(typeof(Lazy<,>));
        }

        public override object GetService(IAmbientServices parent, Type serviceType)
        {
            var descriptors = this.GetServiceDescriptors(parent, serviceType);
            var (_, factory) = descriptors.SingleOrDefault();
            if (factory == null)
            {
                var innerType = serviceType.GetGenericArguments()[0];
                throw new InjectionException(Strings.NoImplementationForServiceType_Exception.FormatWith(innerType));
            }

            return factory();
        }

        public override IEnumerable<(IServiceInfo serviceInfo, Func<object> factory)> GetServiceDescriptors(
            IAmbientServices parent,
            Type serviceType)
        {
            var genericArgs = serviceType.GetGenericArguments();
            var innerType = genericArgs[0];
            var metadataType = genericArgs[1];
            var getService = GetServiceMethod.MakeGenericMethod(innerType, metadataType);
            return this.GetServiceDescriptors(parent, innerType, ((IServiceInfo serviceInfo, Func<object> fn) tuple) => () => getService.Call(null, this.typeRegistry, this.metadataResolver, tuple.serviceInfo, tuple.fn));
        }

        private static Lazy<T, TMetadata> GetService<T, TMetadata>(IRuntimeTypeRegistry typeRegistry, IAppServiceMetadataResolver metadataResolver, IServiceInfo serviceInfo, Func<object> factory)
            where T : class
        {
            return new Lazy<T, TMetadata>(() => (T)factory(), metadataResolver.GetMetadata<TMetadata>(typeRegistry, serviceInfo));
        }
    }
}
