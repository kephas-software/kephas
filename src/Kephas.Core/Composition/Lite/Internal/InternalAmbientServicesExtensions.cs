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
    using Kephas.Composition;
    using Kephas.Reflection;
    using Kephas.Services.Composition;

    internal static class InternalAmbientServicesExtensions
    {
        internal static ICompositionContext AsCompositionContext(this IAmbientServices ambientServices)
        {
            const string AsCompositionContextKey = "__AsCompositionContext";
            if (ambientServices[AsCompositionContextKey] is ICompositionContext compositionContext)
            {
                return compositionContext;
            }

            compositionContext = ambientServices.ToCompositionContext();
            ambientServices[AsCompositionContextKey] = compositionContext;
            return compositionContext;
        }

        internal static TMetadata GetMetadata<TMetadata>(
            this IAppServiceMetadataResolver metadataResolver,
            IServiceInfo serviceInfo)
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
                    metadata = serviceInfo.Metadata == null
                                   ? new Dictionary<string, object>()
                                   : new Dictionary<string, object>(serviceInfo.Metadata);

                    metadata.Add(nameof(AppServiceMetadata.AppServiceImplementationType), serviceInfo.InstanceType);

                    AddMetadataFromGenericServiceType(metadata, metadataResolver, serviceInfo.ServiceType, serviceInfo.InstanceType);
                    AddMetadataFromAttributes(metadata, metadataResolver, serviceInfo.InstanceType);

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
            IAppServiceMetadataResolver metadataResolver,
            Type serviceImplementationType)
        {
            // leave the conversion to RuntimeTypeInfo here
            // as it may be possible to use runtime added attributes
            var metaAttrs = serviceImplementationType.AsRuntimeTypeInfo().GetAttributes<Attribute>();
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