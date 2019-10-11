// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LazyWithMetadataServiceSource.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the lazy with metadata service source class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Lite.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Reflection;
    using Kephas.Services.Composition;

    internal class LazyWithMetadataServiceSource : ServiceSourceBase
    {
        private static readonly MethodInfo GetServiceMethod =
            ReflectionHelper.GetGenericMethodOf(_ => LazyWithMetadataServiceSource.GetService<string, string>(null, null, null));

        private readonly IAppServiceMetadataResolver metadataResolver;

        public LazyWithMetadataServiceSource(IServiceRegistry registry)
            : base(registry)
        {
            this.metadataResolver = new AppServiceMetadataResolver();
        }

        public override bool IsMatch(Type contractType)
        {
            return contractType.IsConstructedGenericOf(typeof(Lazy<,>));
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
            var genericArgs = serviceType.GetGenericArguments();
            var innerType = genericArgs[0];
            var metadataType = genericArgs[1];
            var getService = GetServiceMethod.MakeGenericMethod(innerType, metadataType);
            return this.GetServiceDescriptors(parent, innerType, ((IServiceInfo serviceInfo, Func<object> fn) tuple) => () => getService.Call(null, this.metadataResolver, tuple.serviceInfo, tuple.fn));
        }

        private static Lazy<T, TMetadata> GetService<T, TMetadata>(IAppServiceMetadataResolver metadataResolver, IServiceInfo serviceInfo, Func<object> factory)
            where T : class
        {
            return new Lazy<T, TMetadata>(() => (T)factory(), metadataResolver.GetMetadata<TMetadata>(serviceInfo));
        }
    }
}
