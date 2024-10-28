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
internal class TypesServiceInfoProvider : IServiceInfoProvider
{
    private readonly List<Type> types;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypesServiceInfoProvider"/> class.
    /// </summary>
    /// <param name="types">The types.</param>
    public TypesServiceInfoProvider(IEnumerable<Type> types) => this.types = types?.ToList() ?? throw new ArgumentNullException(nameof(types));

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
        where type.IsEligibleImplementationType(includeInternal: true)
        let serviceDeclarationType = TryGetServiceDeclarationType(type)
        where serviceDeclarationType != null
        select new ServiceImplementationDeclaration(type, serviceDeclarationType!);

    private Type? TryGetServiceDeclarationType(Type type)
    {
        if (IsServiceDeclarationType(type))
        {
            return GetOriginalServiceDeclarationType(type);
        }

        var serviceDeclarationType = type.GetInterfaces()
            .Select(t => t.ToNormalizedType())
            .FirstOrDefault(IsServiceDeclarationType);
        if (serviceDeclarationType != null)
        {
            return GetOriginalServiceDeclarationType(serviceDeclarationType);
        }

        var baseType = type.BaseType?.ToNormalizedType();
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

        var serviceInfo = ((IServiceInfoProvider)this).TryGetServiceInfo(type);
        if (serviceInfo == null)
        {
            return type;
        }

        return serviceInfo.ServiceType is { IsGenericType: false }
            ? type.GetGenericTypeDefinition()
            : type;
    }

    private bool IsServiceDeclarationType(Type type) =>
        (type.IsAbstract && types.Contains(type)) ||
        ((IServiceInfoProvider)this).TryGetServiceInfo(type) != null;
}
