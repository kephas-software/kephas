// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LazyService.cs" company="Kephas Software SRL">
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
/// <typeparam name="TImplementation">The service implementation type.</typeparam>
public class LazyService<T, TImplementation> : Lazy<T>
    where TImplementation : class, T
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LazyService{T, TImplementation}"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public LazyService(IServiceProvider serviceProvider)
        : base(() => serviceProvider.GetRequiredService<TImplementation>())
    {
    }
}

/// <summary>
/// Specialization of <see cref="Lazy{T, TMetadata}"/> retrieving the value from the service provider.
/// </summary>
/// <typeparam name="T">The contract type.</typeparam>
/// <typeparam name="TImplementation">The service implementation type.</typeparam>
/// <typeparam name="TMetadata">The metadata type.</typeparam>
public class LazyService<T, TImplementation, TMetadata> : Lazy<T, TMetadata>
    where TImplementation : class, T
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LazyService{T, TImplementation, TMetadata}"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="metadata">The metadata.</param>
    internal LazyService(IServiceProvider serviceProvider, IDictionary<string, object?>? metadata)
        : base(
            () => serviceProvider.GetRequiredService<TImplementation>(),
            ServiceHelper.GetServiceMetadata<TMetadata>(metadata))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LazyService{T, TImplementation, TMetadata}"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public LazyService(IServiceProvider serviceProvider)
        : base(
            () => serviceProvider.GetRequiredService<TImplementation>(),
            ServiceHelper.GetServiceMetadata<TMetadata>(typeof(TImplementation), typeof(T)))
    {
    }
}