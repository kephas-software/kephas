// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypesAppServiceInfoProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using Kephas.DependencyInjection;
using Kephas.DependencyInjection.ServiceProviders;

namespace Kephas.Services.Builder;

/// <summary>
/// Implementation of <see cref="IServiceInfoProvider"/> for an enumeration of types.
/// </summary>
internal class TypesServiceInfoProvider : IServiceInfoProvider
{
    private readonly IList<Type> types;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypesServiceInfoProvider"/> class.
    /// </summary>
    /// <param name="types">The types.</param>
    public TypesServiceInfoProvider(IEnumerable<Type> types)
    {
        this.types = types.ToList();
    }

    /// <summary>
    /// Gets the contract declaration types.
    /// </summary>
    /// <returns>
    /// The contract declaration types.
    /// </returns>
    IEnumerable<ServiceDeclaration>? IServiceInfoProvider.GetServiceDeclarations() => types.Select(p => new ServiceDeclaration(p, null));

    /// <summary>
    /// Gets an enumeration of tuples containing the service type and the contract declaration type which it implements.
    /// </summary>
    /// <returns>
    /// An enumeration of tuples containing the service type and the contract declaration type which it implements.
    /// </returns>
    public IEnumerable<ServiceImplementationDeclaration> GetImplementationDeclarations() =>
        from type in types
        where type.IsClass && !type.IsAbstract && !type.IsNestedPrivate && type.GetCustomAttribute<ExcludeFromDependencyInjectionAttribute>() == null
        let serviceDeclarationType = TryGetServiceDeclarationType(type)
        where serviceDeclarationType != null
        select new ServiceImplementationDeclaration(type, serviceDeclarationType!);

    private Type? TryGetServiceDeclarationType(Type type)
    {
        if (IsServiceDeclarationType(type))
        {
            return GetOriginalServiceDeclarationType(type);
        }

        var serviceDeclarationType = type.GetInterfaces().FirstOrDefault(IsServiceDeclarationType);
        if (serviceDeclarationType != null)
        {
            return GetOriginalServiceDeclarationType(serviceDeclarationType);
        }

        var baseType = type.BaseType;
        while (baseType != null)
        {
            if (IsServiceDeclarationType(baseType))
            {
                return GetOriginalServiceDeclarationType(baseType);
            }

            baseType = baseType.BaseType;
        }

        return null;
    }

    private Type GetOriginalServiceDeclarationType(Type type)
    {
        if (!type.IsGenericType)
        {
            return type;
        }

        var serviceAttr = type.GetCustomAttribute<ServiceAttributeBase>();
        if (serviceAttr == null)
        {
            return type;
        }

        return serviceAttr.ServiceType is { IsGenericType: false }
            ? type.GetGenericTypeDefinition()
            : type;
    }

    private bool IsServiceDeclarationType(Type type) =>
        ((IServiceInfoProvider)this).TryGetServiceInfo(type) != null;
}
