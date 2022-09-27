// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceProviderInjectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Kephas.Injection;
using Kephas.Resources;
using Kephas.Services;

/// <summary>
/// Extension methods for <see cref="IServiceProvider"/>.
/// </summary>
public static class ServiceProviderInjectionExtensions
{
    /// <summary>
    /// Resolves the specified contract type.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="contractType">Type of the contract.</param>
    /// <returns>An object implementing <paramref name="contractType"/>.</returns>
    public static object Resolve(this IServiceProvider serviceProvider, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType)
    {
        serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        return serviceProvider.GetRequiredService(contractType);
    }

    /// <summary>
    /// Resolves the specified contract type returning multiple instances.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="contractType">Type of the contract.</param>
    /// <returns>An enumeration of objects implementing <paramref name="contractType"/>.</returns>
    public static IEnumerable<object> ResolveMany(this IServiceProvider serviceProvider, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType)
        => (IEnumerable<object>)Resolve(
            serviceProvider,
            typeof(IEnumerable<>).MakeGenericType(
                contractType ??
                throw new ArgumentNullException(nameof(contractType))));

    /// <summary>
    /// Resolves the specified contract type.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>
    /// An object implementing <typeparamref name="T" />.
    /// </returns>
    [return: NotNull]
    public static T Resolve<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(this IServiceProvider serviceProvider)
        where T : class
        => (T)Resolve(serviceProvider, typeof(T));

    /// <summary>
    /// Tries to resolve the service with the provided name.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="serviceName">Name of the service.</param>
    /// <returns>
    /// The named service.
    /// </returns>
    public static T? TryResolve<T>(this IServiceProvider serviceProvider, string serviceName)
    {
        var exportFactories = Resolve<ILazyEnumerable<T, AppServiceMetadata>>(serviceProvider)
            .Where(f => f.Metadata.ServiceName == serviceName)
            .ToList();
        return exportFactories.Count switch
        {
            0 => default,
            > 1 => throw new AmbiguousMatchException(
                string.Format(
                    AbstractionStrings.DefaultNamedServiceProvider_GetNamedService_AmbiguousMatch_Exception,
                    serviceName,
                    typeof(T))),
            _ => exportFactories[0].Value
        };
    }

    /// <summary>
    /// Resolves the service with the provided name.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="serviceName">Name of the service.</param>
    /// <returns>
    /// The named service.
    /// </returns>
    [return: NotNull]
    public static T Resolve<T>(this IServiceProvider serviceProvider, string serviceName)
    {
        var service = TryResolve<T>(serviceProvider, serviceName);
        return service is null
            ? throw new ServiceException(
                string.Format(
                    AbstractionStrings.DefaultNamedServiceProvider_GetNamedService_NoServiceFound_Exception,
                    serviceName,
                    typeof(T)))
            : service;
    }

    /// <summary>
    /// Resolves the specified contract type returning multiple instances.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>
    /// An enumeration of objects implementing <typeparamref name="T" />.
    /// </returns>
    public static IEnumerable<T> ResolveMany<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(this IServiceProvider serviceProvider)
        where T : class =>
        serviceProvider.GetRequiredService<IEnumerable<T>>();

    /// <summary>
    /// Tries to resolve the specified contract type.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="contractType">Type of the contract.</param>
    /// <returns>An object implementing <paramref name="contractType"/>, or <c>null</c> if a service with the provided contract was not found.</returns>
    public static object? TryResolve(this IServiceProvider serviceProvider, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType) =>
        serviceProvider.GetService(contractType);

    /// <summary>
    /// Tries to resolve the specified contract type.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>
    /// An object implementing <typeparamref name="T" />, or <c>null</c> if a service with the provided contract was not found.
    /// </returns>
    public static T? TryResolve<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(this IServiceProvider serviceProvider)
        where T : class
        => serviceProvider.GetService<T>();

    /// <summary>
    /// Creates a disposable injection scope.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>
    /// The new disposable injection scope.
    /// </returns>
    public static IInjectionScope CreateScope(this IServiceProvider serviceProvider) =>
        serviceProvider.GetRequiredService<IInjectionScopeFactory>().CreateScope();
}