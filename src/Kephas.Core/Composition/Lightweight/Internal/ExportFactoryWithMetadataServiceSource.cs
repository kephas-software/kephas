// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportFactoryWithMetadataServiceSource.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the export factory with metadata service source class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Lightweight.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
    using Kephas.Reflection;
    using Kephas.Services.Composition;

    internal class ExportFactoryWithMetadataServiceSource : ServiceSourceBase
    {
        private static readonly MethodInfo GetServiceMethod =
            ReflectionHelper.GetGenericMethodOf(_ => ExportFactoryWithMetadataServiceSource.GetService<string, string>(null, null, null));

        private IAppServiceMetadataResolver metadataResolver;

        public ExportFactoryWithMetadataServiceSource(IServiceRegistry registry)
            : base(registry)
        {
            this.metadataResolver = new AppServiceMetadataResolver();
        }

        public override bool IsMatch(Type contractType)
        {
            return contractType.IsConstructedGenericOf(typeof(IExportFactory<,>));
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

        private static IExportFactory<T, TMetadata> GetService<T, TMetadata>(IAppServiceMetadataResolver metadataResolver, IServiceInfo serviceInfo, Func<object> factory)
            where T : class
        {
            return new ExportFactory<T, TMetadata>(() => (T)factory(), GetMetadata<TMetadata>(metadataResolver, serviceInfo));
        }

        private static TMetadata GetMetadata<TMetadata>(IAppServiceMetadataResolver metadataResolver, IServiceInfo serviceInfo)
        {
            IDictionary<string, object> metadata;
            if (serviceInfo.InstanceType != null)
            {
                const string AppServiceMetadataKey = "__AppServiceMetadata";
                var instanceTypeInfo = serviceInfo.InstanceType.AsRuntimeTypeInfo();
                if (instanceTypeInfo[AppServiceMetadataKey] is IDictionary<string, object> savedMetadata)
                {
                    metadata = savedMetadata;
                }
                else
                {
                    metadata = CreateMetadata(metadataResolver, serviceInfo.InstanceType);
                    instanceTypeInfo[AppServiceMetadataKey] = metadata;
                }
            }
            else
            {
                metadata = new Dictionary<string, object>();
            }

            return (TMetadata)typeof(TMetadata).AsRuntimeTypeInfo()
                .CreateInstance(new object[] { metadata });
        }

        private static IDictionary<string, object> CreateMetadata(
            IAppServiceMetadataResolver metadataResolver,
            Type serviceImplementationType)
        {
            var metadata = new Dictionary<string, object>();
            metadata.Add(nameof(AppServiceMetadata.AppServiceImplementationType), serviceImplementationType);

            var metaAttrs = serviceImplementationType.GetCustomAttributes(inherit: true);
            foreach (var metaAttr in metaAttrs)
            {
                var attrType = metaAttr.GetType();
                var attrTypeInfo = attrType.AsRuntimeTypeInfo();
                var valueProperties = metadataResolver.GetMetadataValueProperties(attrTypeInfo);
                foreach (var valuePropertyEntry in valueProperties)
                {
                    metadata.Add(
                        valuePropertyEntry.Key,
                        metadataResolver.GetMetadataValueFromAttribute(
                            serviceImplementationType,
                            attrType,
                            valuePropertyEntry.Value));
                }
            }

            return metadata;
        }
    }
}