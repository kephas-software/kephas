// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpenGenericLazyService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection;

using Kephas.Services;

/// <summary>
/// Specialization of <see cref="Lazy{T}"/> retrieving the value from the service provider.
/// </summary>
/// <typeparam name="T">The contract type.</typeparam>
public class OpenGenericLazyService<T> : Lazy<T>
    where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenGenericLazyService{T}"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public OpenGenericLazyService(IServiceProvider serviceProvider)
        : base(serviceProvider.GetRequiredService<T>)
    {
    }
}

/// <summary>
/// Specialization of <see cref="Lazy{T, TMetadata}"/> retrieving the value from the service provider.
/// </summary>
/// <typeparam name="T">The contract type.</typeparam>
/// <typeparam name="TImplementation">The service implementation type.</typeparam>
/// <typeparam name="TMetadata">The metadata type.</typeparam>
public class OpenGenericLazyService<T, TImplementation, TMetadata> : Lazy<T, TMetadata>
    where T : class
    where TImplementation : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenGenericLazyService{T, TImplementation, TMetadata}"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="metadata">The metadata.</param>
    internal OpenGenericLazyService(IServiceProvider serviceProvider, IDictionary<string, object?>? metadata)
        : base(
            serviceProvider.GetRequiredService<T>,
            ServiceHelper.GetServiceMetadata<TMetadata>(metadata))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenGenericLazyService{T, TImplementation, TMetadata}"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public OpenGenericLazyService(IServiceProvider serviceProvider)
        : base(
            serviceProvider.GetRequiredService<T>,
            ServiceHelper.GetServiceMetadata<TMetadata>(typeof(TImplementation), typeof(T)))
    {
    }
}