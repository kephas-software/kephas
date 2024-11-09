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
using System.Runtime.CompilerServices;
using Kephas.DependencyInjection.Metadata;

namespace Kephas.DependencyInjection;

/// <summary>
/// Provides helper methods for service registration.
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
        var ctor = metadataType.GetConstructor([typeof(IDictionary<string, object?>)]);
        if (ctor is not null)
        {
            return (TMetadata)ctor.Invoke([metadata]);
        }

        ctor = metadataType.GetConstructor(Type.EmptyTypes);
        if (ctor is not null)
        {
            var typedMetadata = ctor.Invoke([]);
            if (metadata is not null)
            {
                // TODO optimize
                var setters = metadataType
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.CanWrite)
                    .ToDictionary(p => p.Name, p => (Action<object?, object?>)(p.SetValue));
                foreach (var kv in metadata)
                {
                    if (setters.TryGetValue(kv.Key, out var setter))
                    {
                        setter(typedMetadata, kv.Value);
                    }
                }
            }

            return (TMetadata)typedMetadata;
        }

        throw new ServiceException($"Cannot instantiate metadata of type {typeof(TMetadata)}, neither a constructor accepting an IDictionary<string, object> parameter, nor a default constructor is provided.");
    }

    /// <summary>
    /// Collects the metadata from the service type attributes and generic type arguments.
    /// </summary>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    /// <param name="implementationType">The implementation type.</param>
    /// <param name="serviceDeclarationType">The service declaration type.</param>
    /// <returns>The typed metadata.</returns>
    public static TMetadata GetServiceMetadata<TMetadata>(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? implementationType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceDeclarationType) =>
        GetServiceMetadata<TMetadata>(GetServiceMetadata(implementationType, serviceDeclarationType));

    /// <summary>
    /// Collects the metadata from the service type attributes and generic type arguments.
    /// </summary>
    /// <param name="implementationType">The implementation type.</param>
    /// <param name="serviceDeclarationType">The service declaration type.</param>
    /// <returns>The metadata.</returns>
    public static IDictionary<string, object?> GetServiceMetadata(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? implementationType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceDeclarationType)
    {
        var metadata = new Dictionary<string, object?>();

        if (implementationType is not null)
        {
            foreach (var m in implementationType.GetCustomAttributes()
                         .OfType<IServiceMetadataProvider>()
                         .SelectMany(p => p.GetMetadata()))
            {
                metadata[m.Key] = m.Value;
            }
        }

        metadata.Add(nameof(ServiceMetadata.ImplementationType), implementationType);
        if (implementationType is not null && !metadata.ContainsKey(nameof(ServiceMetadata.ServiceKey)))
        {
            var implementationName = implementationType.Name;
            metadata[nameof(ServiceMetadata.ServiceKey)] =
                TryGetServiceKey(implementationName, serviceDeclarationType)
                ?? implementationName;
        }

        // add metadata from generic parameters
        return AddGenericServiceMetadata(metadata, implementationType, serviceDeclarationType);
    }

    /// <summary>
    /// Collects the metadata from the service type attributes and generic type arguments.
    /// </summary>
    /// <param name="metadata">The service metadata.</param>
    /// <param name="implementationType">The implementation type.</param>
    /// <param name="serviceDeclarationType">The service declaration type.</param>
    /// <returns>The metadata.</returns>
    public static IDictionary<string, object?> AddGenericServiceMetadata(
        IDictionary<string, object?> metadata,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? implementationType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceDeclarationType)
    {
        ArgumentNullException.ThrowIfNull(nameof(metadata));

        // add metadata from generic parameters
        if (serviceDeclarationType.IsGenericType)
        {
            var metadataSourceGenericType = serviceDeclarationType.IsConstructedGenericType
                ? serviceDeclarationType
                : implementationType is null || implementationType.IsGenericType
                    ? null
                    : implementationType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == serviceDeclarationType);
            if (metadataSourceGenericType != null)
            {
                foreach (var m in ServiceMetadataHelper
                             .GetGenericTypeMetadataProvider(metadataSourceGenericType)
                             .GetMetadata())
                {
                    metadata[m.Key] = m.Value;
                }
            }
        }

        return metadata;
    }

    /// <summary>
    /// Converts the <see cref="RegistrationKind"/> to a nullable boolean.
    /// </summary>
    /// <param name="registrationKind">The value to convert.</param>
    /// <param name="serviceType">The service type.</param>
    /// <returns><c>null</c> for non-generic types, <c>false</c> for constructed generic types, <c>true</c> for open generic types if not set <see cref="RegistrationKind.AsClosedGeneric"/>, otherwise <c>false</c>.</returns>
    public static bool? ToNullableBoolean(this RegistrationKind registrationKind, Type? serviceType) =>
        serviceType?.IsGenericType is not true
            ? null
            : serviceType.IsConstructedGenericType
                ? false
                : registrationKind is RegistrationKind.Default or RegistrationKind.AsOpenGeneric;

    private static string? TryGetServiceKey(string implementationName, Type? serviceType)
    {
        if (serviceType is null)
        {
            return null;
        }
        
        var suffix = serviceType.Name;
        if (suffix.StartsWith('I') && serviceType.IsInterface)
        {
            suffix = suffix[1..];
        }

        suffix = StripGenericSuffix(suffix);
        implementationName = StripGenericSuffix(implementationName);

        return implementationName.EndsWith(suffix) && implementationName.Length > suffix.Length
            ? implementationName[..^(suffix.Length)]
            : null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string StripGenericSuffix(string typeName)
    {
        var genericIndex = typeName.IndexOf('`');
        return genericIndex > 0 ? typeName[..genericIndex] : typeName;
    }
}
