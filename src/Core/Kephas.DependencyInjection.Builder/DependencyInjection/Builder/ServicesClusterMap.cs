// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServicesClusterMap.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Reflection;
using Kephas.DependencyInjection.ServiceProviders;
using Kephas.Reflection;

namespace Kephas.DependencyInjection.Builder;

internal class ServicesClusterMap(Action<string, Type, string>? logger = null)
{
    private const string WarningLevel = "Warning";
    private const string ErrorLevel = "Error";

    private readonly Dictionary<Type, ServicesCluster> map = new();

    public ServicesCluster AddServiceDeclarationInfo(ServiceDeclarationInfo serviceDeclarationInfo)
    {
        var serviceType = serviceDeclarationInfo.ServiceInfo.ServiceType ?? serviceDeclarationInfo.ServiceDeclarationType;
        var cluster = new ServicesCluster(serviceType, serviceDeclarationInfo.ServiceInfo);
        var serviceDeclarationType = serviceDeclarationInfo.ServiceDeclarationType;
        if (map.TryGetValue(serviceType, out var existingCluster))
        {
            logger?.Invoke(WarningLevel, serviceType, $"{nameof(AddServiceDeclarationInfo)}: Duplicate registration for the same service type {serviceType}.");
            return existingCluster;
        }

        if (map.TryGetValue(serviceDeclarationType, out existingCluster))
        {
            logger?.Invoke(WarningLevel, serviceDeclarationType, $"{nameof(AddServiceDeclarationInfo)}: Duplicate registration for the same service type {serviceDeclarationType}.");
            return existingCluster;
        }

        map[serviceType] = cluster;
        if(serviceType != serviceDeclarationType)
        {
            map[serviceDeclarationType] = cluster;
        }

        return cluster;
    }

    public void AddServiceImplementationDeclaration(ServiceImplementationDeclaration serviceImplementationDeclaration)
    {
        var serviceDeclarationType = serviceImplementationDeclaration.ServiceDeclarationType;
        var implementationType = serviceImplementationDeclaration.ImplementationType;
        
        // in case:
        // * constructed generic or non-generic implementation with open generic service type
        // normalize the service type to the constructed service type matching to the implementation
        if (serviceDeclarationType.IsGenericTypeDefinition &&
            (implementationType.IsConstructedGenericType || !implementationType.IsGenericType))
        {
            var serviceDeclarationTypeGenericDefinition = serviceDeclarationType;
            serviceDeclarationType =
                implementationType.TryGetBaseConstructedGenericOf(serviceDeclarationTypeGenericDefinition)
                ?? throw new InvalidOperationException($"The service declaration type {serviceDeclarationTypeGenericDefinition} does not have any constructed type that the implementation type {implementationType} can be converted to.");
        }

        // first try to get the cluster of the declaration type.
        if (map.TryGetValue(serviceDeclarationType, out var servicesCluster))
        {
            // the simplest cases:
            // * non-generic service type, non-generic implementation type
            // * generic definition service type, generic definition implementation type (opn registration)
            servicesCluster.AddRegistration(serviceDeclarationType, implementationType);
            return;
        }

        // if not found, this might be in case of generics, a constructed generic is provided.
        if (!serviceDeclarationType.IsConstructedGenericType)
        {
            logger?.Invoke(ErrorLevel, serviceDeclarationType, $"Declarations not found for {serviceDeclarationType}, cannot proceed to register implementation {implementationType}.");
            return;
        }

        // generic case:
        // * generic definition service declaration type (closed registration), constructed generic implementation type
        // * generic definition service declaration type (closed registration), non-generic implementation type
        var openGenericDeclarationType = serviceDeclarationType.GetGenericTypeDefinition();
        if (map.TryGetValue(openGenericDeclarationType, out var openGenericServicesCluster))
        {
            if (openGenericServicesCluster.ServiceType.IsGenericType)
            {
                // the service type is generic, it must be an open generic.
                // add the cluster for the closed generic service type
                // and in the end add the registration
                servicesCluster = AddServiceDeclarationInfo(new ServiceDeclarationInfo(
                    serviceDeclarationType,
                    new ServiceInfo(openGenericServicesCluster.ServiceInfo, serviceDeclarationType, serviceDeclarationType)));
                servicesCluster.AddRegistration(serviceDeclarationType, implementationType);
                return;
            }

            // the service type is non-generic, try to get the service cluster for the service type
            // if not found, create it.
            if (!map.TryGetValue(openGenericServicesCluster.ServiceType, out servicesCluster))
            {
                servicesCluster = AddServiceDeclarationInfo(new ServiceDeclarationInfo(
                    serviceDeclarationType,
                    new ServiceInfo(openGenericServicesCluster.ServiceInfo, openGenericServicesCluster.ServiceType, serviceDeclarationType)));
            }

            servicesCluster.AddRegistration(serviceDeclarationType, implementationType);
        }
            
        // in case a generic definition cluster is not found, it may be simply that the declaration part is missing
        logger?.Invoke(ErrorLevel, serviceDeclarationType, $"Declarations not found for {serviceDeclarationType}, cannot proceed to register implementation {implementationType}.");
    }
    
    public IEnumerable<ServicesCluster> GetClusters() => map.Values.Distinct();
}