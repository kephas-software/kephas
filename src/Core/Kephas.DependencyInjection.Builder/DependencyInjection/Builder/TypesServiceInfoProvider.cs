// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypesAppServiceInfoProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.ServiceProviders;
using Kephas.Reflection;

namespace Kephas.DependencyInjection.Builder;

/// <summary>
/// Implementation of <see cref="IServiceInfoProvider"/> for an enumeration of types.
/// </summary>
[ExcludeFromDependencyInjection]
internal class TypesServiceInfoProvider(IEnumerable<Type> types, bool includeInternal) : IServiceInfoProvider
{
    private readonly List<Type> types = types?.ToList() ?? throw new ArgumentNullException(nameof(types));

    /// <summary>
    /// Gets the service declaration types.
    /// </summary>
    /// <returns>
    /// The service declaration types.
    /// </returns>
    IEnumerable<ServiceDeclaration>? IServiceInfoProvider.GetServiceDeclarations() =>
        types.Where(IsServiceDeclarationType)
            .Select(p => new ServiceDeclaration(p, null));

    /// <summary>
    /// Gets an enumeration of tuples containing the service type and the service declaration type which it implements.
    /// </summary>
    /// <param name="eligibleImplementationTypes"></param>
    /// <returns>
    /// An enumeration of tuples containing the service type and the service declaration type which it implements.
    /// </returns>
    public IEnumerable<ServiceImplementationDeclaration> GetImplementationDeclarations(IEnumerable<Type> eligibleImplementationTypes) =>
        from type in types
        where type.IsEligibleImplementationType(includeInternal: includeInternal)
        let serviceDeclarationType = TryGetServiceDeclarationType(type)
        where serviceDeclarationType != null
        select new ServiceImplementationDeclaration(type, serviceDeclarationType!);

    private Type? TryGetServiceDeclarationType(Type implementationType)
    {
        if (IsServiceDeclarationType(implementationType))
        {
            return GetOriginalServiceDeclarationType(implementationType);
        }

        var serviceDeclarationType = implementationType.GetInterfaces().Select(t => t.ToNormalizedType())
            .FirstOrDefault(IsServiceDeclarationType);
        if (serviceDeclarationType != null)
        {
            return GetOriginalServiceDeclarationType(serviceDeclarationType);
        }

        var baseType = implementationType.BaseType?.ToNormalizedType();
        while (baseType != null)
        {
            if (IsServiceDeclarationType(baseType))
            {
                return GetOriginalServiceDeclarationType(baseType);
            }

            baseType = baseType.BaseType?.ToNormalizedType();
        }

        return null;
    }

    private Type GetOriginalServiceDeclarationType(Type type)
    {
        if (!type.IsGenericType)
        {
            return type;
        }

        var serviceInfo = ((IServiceInfoProvider)this).TryGetServiceDeclarationInfo(type);
        if (serviceInfo == null)
        {
            return type;
        }

        return serviceInfo.ServiceType is { IsGenericType: true }
            ? type.GetGenericTypeDefinition()
            : type;
    }

    private bool IsServiceDeclarationType(Type type) =>
        (type.IsAbstract && types.Contains(type)) ||
        ((IServiceInfoProvider)this).TryGetServiceDeclarationInfo(type) != null;
}
