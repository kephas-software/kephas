// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Runtime.CompilerServices;
using Kephas.DependencyInjection;
using Kephas.DependencyInjection.ServiceProviders;
using Kephas.Reflection;

namespace Kephas;

internal static class DependencyInjectionHelper
{
    public static IEnumerable<Type> GetEligibleImplementationType(Type serviceType, IEnumerable<Assembly> assemblies, bool includeInternal)
        => GetEligibleImplementationTypes(assemblies, includeInternal)
            .Where(serviceType.IsAssignableFrom);

    internal static IEnumerable<Type> GetEligibleImplementationTypes(IEnumerable<Assembly> assemblies, bool includeInternal) =>
        assemblies.SelectMany(assembly => assembly.GetEligibleImplementationTypes(includeInternal));

    internal static IEnumerable<Type> GetEligibleImplementationTypes(this Assembly assembly, bool includeInternal) =>
        (includeInternal ? assembly.GetTypes() : assembly.ExportedTypes)
            .Where(t => t.IsEligibleImplementationType(includeInternal));

    internal static bool IsEligibleImplementationType(this Type type, bool includeInternal) =>
        (includeInternal
            ? type is { IsClass: true, IsAbstract: false } && (type.IsPublic || type.IsNestedPublic || IsInternalNonCompilerGenerated(type))
            : type is { IsClass: true, IsAbstract: false } && (type.IsPublic || type.IsNestedPublic)) &&
        !typeof(Attribute).IsAssignableFrom(type) &&
        !type.IsDefined(typeof(ExcludeFromDependencyInjectionAttribute));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsInternalNonCompilerGenerated(Type type) =>
        // Checking for CompilerGeneratedAttribute to exclude anonymous types is not always reliable, as the compiler may generate other types as well.
        // Therefore, check for language invalid characters like < in the type name. 
        (!type.IsNested || type.IsNestedAssembly || type.IsNestedFamORAssem) &&
        type.Name.Length > 0 &&
        type.Name[0] != '<' &&
        !type.IsDefined(typeof(CompilerGeneratedAttribute));

    internal static IEnumerable<ServiceImplementationDeclaration> GetImplementationDeclarations(this Type serviceDeclarationType, Type serviceType, IEnumerable<Type> eligibleImplementationTypes) =>
        serviceDeclarationType.IsGenericTypeDefinition
            ? eligibleImplementationTypes
                .Select(t => (implementationType: t, closedServiceType: t.TryGetBaseConstructedGenericOf(serviceDeclarationType)))
                .Where(tuple => tuple.closedServiceType is not null && (serviceType.IsGenericTypeDefinition || serviceType.IsAssignableFrom(tuple.implementationType)))
                .Select(tuple => new ServiceImplementationDeclaration(tuple.implementationType, serviceDeclarationType))
            : eligibleImplementationTypes
                .Where(t => !t.IsGenericTypeDefinition && serviceDeclarationType.IsAssignableFrom(t) && serviceType.IsAssignableFrom(t))
                .Select(t => new ServiceImplementationDeclaration(t, serviceDeclarationType));
}