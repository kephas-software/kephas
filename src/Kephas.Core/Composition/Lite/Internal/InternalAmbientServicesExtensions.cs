// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InternalAmbientServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the internal ambient services extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Lite.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Collections;
    using Kephas.Composition;
    using Kephas.Runtime;
    using Kephas.Services.Composition;

    internal static class InternalAmbientServicesExtensions
    {
        internal static IInjector AsInjector(this IAmbientServices ambientServices)
        {
            const string AsInjectorKey = "__AsInjector";
            if (ambientServices[AsInjectorKey] is IInjector injector)
            {
                return injector;
            }

            injector = ambientServices.ToCompositionContext();
            ambientServices[AsInjectorKey] = injector;
            return injector;
        }

        internal static TMetadata GetMetadata<TMetadata>(
            this IAppServiceMetadataResolver metadataResolver,
            IRuntimeTypeRegistry typeRegistry,
            IServiceInfo serviceInfo)
        {
            IDictionary<string, object> metadata;
            if (serviceInfo.InstanceType != null)
            {
                const string AppServiceMetadataKey = "__AppServiceMetadata";

                // if multiple service infos are defined for the same type,
                // make sure one does not override the other.
                var metadataKey = $"{AppServiceMetadataKey}_{serviceInfo.GetHashCode()}";
                var instanceTypeInfo = typeRegistry.GetTypeInfo(serviceInfo.InstanceType);
                if (instanceTypeInfo[metadataKey] is IDictionary<string, object> savedMetadata)
                {
                    metadata = savedMetadata;
                }
                else
                {
                    metadata = serviceInfo.Metadata == null
                                   ? new Dictionary<string, object>()
                                   : new Dictionary<string, object>(serviceInfo.Metadata);

                    metadata.Add(nameof(AppServiceMetadata.ServiceInstanceType), serviceInfo.InstanceType);

                    AddMetadataFromGenericServiceType(metadata, metadataResolver, serviceInfo.ServiceType, serviceInfo.InstanceType);
                    AddMetadataFromAttributes(metadata, typeRegistry, metadataResolver, serviceInfo.InstanceType);

                    if (serviceInfo.Metadata != null)
                    {
                        metadata.Merge(serviceInfo.Metadata);
                    }

                    instanceTypeInfo[metadataKey] = metadata;
                }
            }
            else
            {
                metadata = serviceInfo.Metadata ?? new Dictionary<string, object>();
            }

            return (TMetadata)typeRegistry
                .GetTypeInfo(typeof(TMetadata))
                .CreateInstance(new object[] { metadata });
        }

        private static void AddMetadataFromGenericServiceType(
            IDictionary<string, object> metadata,
            IAppServiceMetadataResolver metadataResolver,
            Type serviceType,
            Type serviceImplementationType)
        {
            if (!serviceType.IsGenericTypeDefinition)
            {
                return;
            }

            var genericTypeParameters = serviceType.GetTypeInfo().GenericTypeParameters;
            for (var i = 0; i < genericTypeParameters.Length; i++)
            {
                var genericTypeParameter = genericTypeParameters[i];
                var position = i;
                metadata.Add(
                    metadataResolver.GetMetadataNameFromGenericTypeParameter(genericTypeParameter),
                    metadataResolver.GetMetadataValueFromGenericParameter(serviceImplementationType, position, serviceType));
            }
        }

        private static void AddMetadataFromAttributes(
            IDictionary<string, object> metadata,
            IRuntimeTypeRegistry typeRegistry,
            IAppServiceMetadataResolver metadataResolver,
            Type serviceImplementationType)
        {
            // leave the conversion to RuntimeTypeInfo here
            // as it may be possible to use runtime added attributes
            var metaAttrs = typeRegistry.GetTypeInfo(serviceImplementationType).GetAttributes<Attribute>();
            foreach (var metaAttr in metaAttrs)
            {
                var attrType = metaAttr.GetType();
                var valueProperties = metadataResolver.GetMetadataValueProperties(attrType);
                foreach (var valuePropertyEntry in valueProperties)
                {
                    metadata.Add(
                        valuePropertyEntry.Key,
                        valuePropertyEntry.Value.GetValue(metaAttr));
                }
            }
        }
    }
}