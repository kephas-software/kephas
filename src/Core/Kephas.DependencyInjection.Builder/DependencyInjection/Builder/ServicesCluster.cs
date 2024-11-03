// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServicesCluster.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Runtime.CompilerServices;
using Kephas.Collections;
using Kephas.DependencyInjection.Metadata;
using Kephas.DependencyInjection.Reflection;
using Kephas.Reflection;

namespace Kephas.DependencyInjection.Builder;

internal record ServicesCluster(Type ServiceType, IServiceInfo ServiceInfo)
{
    public IList<ServiceInfo> Registrations { get; } = new List<ServiceInfo>();
    
    public void AddRegistration(Type serviceDeclarationType, Type implementationType)
    {
        var implementationServiceInfo = new ServiceInfo(ServiceInfo, ServiceType, serviceDeclarationType, implementationType);
        ((IServiceInfo)implementationServiceInfo).AddMetadata(ServiceHelper.GetServiceMetadata(implementationType, serviceDeclarationType));
        Registrations.Add(implementationServiceInfo);
    }
    
    public IList<ServiceInfo> SortRegistrationsDescending()
    {
        if (Registrations.Count < 2)
        {
            return Registrations.ToList();
        }
        
        // leave the implementation with the runtime type info
        // so that it may be possible to use runtime added attributes
        // the registrations should be sorted in descending order,
        // so that the ones with higher priority are registered last,
        // to override the ones with a lower priority
        var overrideChain = Registrations
            .Select(si => (serviceInfo: si, priority: si.GetPriority(), implementationTypeBase: GetImplementationTypeBase(si), overrideBaseImplementation: GetOverrideBaseImplementation(si)))
            .OrderByDescending(item => item.priority)
            .ToList();

        // get the overridden services which should be eliminated
        var overriddenTypes = overrideChain
            .Where(kv => kv is { overrideBaseImplementation: true, implementationTypeBase: not null })
            .Select(kv => kv.implementationTypeBase!.ToNormalizedType())
            .ToList();

        if (overriddenTypes.Count > 0)
        {
            // eliminate the overridden services
            overrideChain = overrideChain
                .Where(kv => !overriddenTypes.Contains(((IServiceInfo)kv.serviceInfo).ImplementationType!))
                .ToList();
        }

        return overrideChain.Select(e => e.serviceInfo).ToList();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Priority GetPriority(IServiceInfo serviceInfo) =>
        (Priority)(serviceInfo.Metadata?.TryGetValue(nameof(ServiceMetadata.Priority)) ?? Priority.Normal);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Type? GetImplementationTypeBase(IServiceInfo serviceInfo) =>
        serviceInfo.ImplementationType?.BaseType;
    
    private static bool GetOverrideBaseImplementation(IServiceInfo serviceInfo) =>
        (bool)(serviceInfo.Metadata?.TryGetValue(nameof(ServiceMetadata.OverrideBaseImplementation)) ?? false);
}