// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceInfoProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Kephas.DependencyInjection.Reflection;

namespace Kephas.DependencyInjection.ServiceProviders;

/// <summary>
/// Interface providing the <see cref="GetServiceInfos"/> method,
/// which collects <see cref="IServiceInfo"/> data together with the contract declaration type.
/// </summary>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public interface IServiceInfoProvider
{
    /// <summary>
    /// Gets an enumeration of application service information objects and their contract declaration type.
    /// The contract declaration type is the type declaring the contract: if the <see cref="ServiceAttributeBase.ServiceType"/>
    /// is not provided, the contract declaration type is also the contract type.
    /// </summary>
    /// <returns>
    /// An enumeration of application service information objects and their contract declaration type.
    /// </returns>
    public IEnumerable<ServiceDeclarationInfo> GetServiceInfos()
        => GetServiceInfos(this);

    /// <summary>
    /// Gets an enumeration of <see cref="ServiceImplementationDeclaration"/>.
    /// </summary>
    /// <returns>
    /// An enumeration of <see cref="ServiceImplementationDeclaration"/>.
    /// </returns>
    public IEnumerable<ServiceImplementationDeclaration> GetImplementationDeclarations()
        => [];

    /// <summary>
    /// Tries the get the application service information from the custom attributes.
    /// </summary>
    /// <param name="serviceDeclarationType">The service declaration type.</param>
    /// <returns>The <see cref="IServiceInfo"/> instance or <c>null</c>.</returns>
    protected internal IServiceInfo? TryGetServiceInfo(Type serviceDeclarationType) => 
        serviceDeclarationType.GetCustomAttributes(inherit: false).OfType<IServiceInfo>().FirstOrDefault();

    /// <summary>
    /// Gets an enumeration of <see cref="ServiceDeclarationInfo"/>.
    /// The <see cref="ServiceDeclarationInfo.ServiceDeclarationType"/> is the type declaring the service:
    /// if the <see cref="ServiceAttributeBase.ServiceType"/> is not provided, the service declaration type is also the service type.
    /// </summary>
    /// <param name="provider">The provider.</param>
    /// <returns>
    /// An enumeration of <see cref="ServiceDeclarationInfo"/>.
    /// </returns>
    protected internal static IEnumerable<ServiceDeclarationInfo> GetServiceInfos(IServiceInfoProvider provider)
    {
        var serviceDeclarationTypes = provider.GetServiceDeclarations();
        if (serviceDeclarationTypes == null)
        {
            yield break;
        }

        foreach (var (serviceDeclarationType, metadataType) in serviceDeclarationTypes)
        {
            var serviceInfo = provider.TryGetServiceInfo(serviceDeclarationType);
            if (serviceInfo == null)
            {
                continue;
            }

            if (serviceInfo.MetadataType is null)
            {
                serviceInfo.MetadataType = metadataType;
            }
            else if (serviceInfo.MetadataType != metadataType)
            {
                // TODO notify about the metadata type difference
            }

            yield return new ServiceDeclarationInfo(serviceDeclarationType, serviceInfo);
        }
    }

    /// <summary>
    /// Gets an enumeration of <see cref="ServiceDeclaration"/>.
    /// </summary>
    /// <returns>
    /// An enumeration of <see cref="ServiceDeclaration"/>.
    /// </returns>
    protected internal IEnumerable<ServiceDeclaration>? GetServiceDeclarations() => null;
}
