// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service helper class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Kephas.Collections;
using Kephas.Services.Metadata;

namespace Kephas.Services;

/// <summary>
/// A service helper.
/// </summary>
public static class ServiceHelper
{
    /// <summary>
    /// Collects the metadata from the service type attributes and generic type arguments.
    /// </summary>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    /// <param name="metadata">The metadata dictionary.</param>
    /// <returns>The typed metadata.</returns>
    public static TMetadata GetServiceMetadata<TMetadata>(IDictionary<string, object?>? metadata)
    {
        var metadataType = typeof(TMetadata);
        var ctor = metadataType.GetConstructor(new[] { typeof(IDictionary<string, object>) });
        if (ctor is not null)
        {
            return (TMetadata)ctor.Invoke(new object?[] { metadata });
        }

        ctor = metadataType.GetConstructor(Type.EmptyTypes);
        if (ctor is not null)
        {
            var typedMetadata = ctor.Invoke(Array.Empty<object>());
            if (metadata is not null)
            {
                // TODO optimize
                var setters = metadataType
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.CanWrite)
                    .ToDictionary(p => p.Name, p => (Action<object?, object?>)(p.SetValue));
                metadata.ForEach(kv =>
                {
                    if (setters.TryGetValue(kv.Key, out var setter))
                    {
                        setter(typedMetadata, kv.Value);
                    }
                });
            }

            return (TMetadata)typedMetadata;
        }

        throw new ServiceException($"Cannot instantiate metadata of type {typeof(TMetadata)}, neither a constructor accepting an IDictionary<string, object> parameter, nor a default constructor is provided.");
    }

    /// <summary>
    /// Collects the metadata from the service type attributes and generic type arguments.
    /// </summary>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    /// <param name="serviceType">The service type.</param>
    /// <param name="contractDeclarationType">The contract declaration type.</param>
    /// <returns>The typed metadata.</returns>
    public static TMetadata GetServiceMetadata<TMetadata>(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? serviceType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractDeclarationType) =>
        GetServiceMetadata<TMetadata>(GetServiceMetadata(serviceType, contractDeclarationType));

    /// <summary>
    /// Collects the metadata from the service type attributes and generic type arguments.
    /// </summary>
    /// <param name="serviceType">The service type.</param>
    /// <param name="contractDeclarationType">The contract declaration type.</param>
    /// <returns>The metadata.</returns>
    public static IDictionary<string, object?> GetServiceMetadata(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? serviceType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractDeclarationType)
    {
        var metadata = new Dictionary<string, object?>();
        serviceType?.GetCustomAttributes()
            .OfType<IServiceMetadataProvider>()
            .SelectMany(p => p.GetMetadata())
            .ForEach(m => metadata[m.name] = m.value);

        metadata.Add(nameof(AppServiceMetadata.ServiceType), serviceType);

        // add metadata from generic parameters
        if (contractDeclarationType.IsGenericType)
        {
            var metadataSourceGenericType = contractDeclarationType.IsConstructedGenericType
                ? contractDeclarationType
                : serviceType is null || serviceType.IsGenericType
                    ? null
                    : serviceType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == contractDeclarationType);
            if (metadataSourceGenericType != null)
            {
                IServiceMetadataProvider.GetGenericTypeMetadataProvider(metadataSourceGenericType)
                    .GetMetadata()
                    .ForEach(m => metadata[m.name] = m.value);
            }
        }

        return metadata;
    }
}
