// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypesAppServiceInfoProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;

namespace Kephas.Services.Builder;

/// <summary>
/// Implementation of <see cref="IAppServiceInfoProvider"/> for an enumeration of types.
/// </summary>
internal class TypesAppServiceInfoProvider : IAppServiceInfoProvider
{
    private readonly IList<Type> types;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypesAppServiceInfoProvider"/> class.
    /// </summary>
    /// <param name="types">The types.</param>
    public TypesAppServiceInfoProvider(IEnumerable<Type> types)
    {
        this.types = types.ToList();
    }

    /// <summary>
    /// Gets the contract declaration types.
    /// </summary>
    /// <returns>
    /// The contract declaration types.
    /// </returns>
    IEnumerable<ContractDeclarationInfo>? IAppServiceInfoProvider.GetContractDeclarationTypes() => types.Select(p => new ContractDeclarationInfo(p, null));

    /// <summary>
    /// Gets an enumeration of tuples containing the service type and the contract declaration type which it implements.
    /// </summary>
    /// <returns>
    /// An enumeration of tuples containing the service type and the contract declaration type which it implements.
    /// </returns>
    public IEnumerable<ServiceDeclaration> GetAppServices()
    {
        return
            from serviceType in types
            where serviceType.IsClass && !serviceType.IsAbstract && !serviceType.IsNestedPrivate && serviceType.GetCustomAttribute<ExcludeFromServicesAttribute>() == null
            let contractDeclarationType = TryGetAppServiceContract(serviceType)
            where contractDeclarationType != null
            select new ServiceDeclaration(serviceType, contractDeclarationType!);
    }

    private Type? TryGetAppServiceContract(Type part)
    {
        if (IsAppServiceContract(part))
        {
            return GetOriginalAppServiceContract(part);
        }

        var contract = part.GetInterfaces().FirstOrDefault(IsAppServiceContract);
        if (contract != null)
        {
            return GetOriginalAppServiceContract(contract);
        }

        var baseType = part.BaseType;
        while (baseType != null)
        {
            if (IsAppServiceContract(baseType))
            {
                return GetOriginalAppServiceContract(baseType);
            }

            baseType = baseType.BaseType;
        }

        return null;
    }

    private Type GetOriginalAppServiceContract(Type type)
    {
        if (!type.IsGenericType)
        {
            return type;
        }

        var appServiceContractAttr = type.GetCustomAttribute<AppServiceContractAttribute>();
        if (appServiceContractAttr == null)
        {
            return type;
        }

        return appServiceContractAttr.ContractType is { IsGenericType: false }
            ? type.GetGenericTypeDefinition()
            : type;
    }

    private bool IsAppServiceContract(Type type) =>
        ((IAppServiceInfoProvider)this).TryGetAppServiceInfo(type) != null;
}
