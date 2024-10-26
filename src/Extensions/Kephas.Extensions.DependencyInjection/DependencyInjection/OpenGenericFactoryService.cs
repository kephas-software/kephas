// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpenGenericFactoryService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Kephas.Extensions.DependencyInjection;

/// <summary>
/// Service returning a <see cref="T"/> service based on an open generic.
/// </summary>
/// <typeparam name="T">The service contract type.</typeparam>
public class OpenGenericFactoryService<T> : IFactory<T>
    where T : class
{
    private readonly Func<T> factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenGenericFactoryService{T}"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public OpenGenericFactoryService(IServiceProvider serviceProvider)
    : this(serviceProvider.GetRequiredService<T>)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenGenericFactoryService{T}"/> class.
    /// </summary>
    /// <param name="factory">The service factory.</param>
    protected internal OpenGenericFactoryService(Func<T> factory)
    {
        this.factory = factory;
    }

    /// <summary>
    /// Gets an instance retrieved from the dependency injection container.
    /// </summary>
    public T GetService() => factory();

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>
    /// A string that represents the current object.
    /// </returns>
    public override string ToString() =>
        $"{GetType().Name} of {typeof(T).Name}.";
}

/// <summary>
/// Service returning a <see cref="T"/> service based on the <see cref="TImplementation"/> with associated <see cref="TMetadata"/>.
/// </summary>
/// <typeparam name="T">The service contract type.</typeparam>
/// <typeparam name="TImplementation">The service implementation type.</typeparam>
/// <typeparam name="TMetadata">The metadata type.</typeparam>
public class OpenGenericFactoryService<T, TImplementation, TMetadata> : OpenGenericFactoryService<T>, IFactory<T, TMetadata>
    where T : class
    where TImplementation : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenGenericFactoryService{T,TImplementation,TMetadata}"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public OpenGenericFactoryService(IServiceProvider serviceProvider)
        : this(
            serviceProvider.GetRequiredService<T>,
            ServiceHelper.GetServiceMetadata<TMetadata>(typeof(TImplementation), typeof(T)))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenGenericFactoryService{T,TImplementation,TMetadata}"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="metadata">The metadata.</param>
    internal OpenGenericFactoryService(IServiceProvider serviceProvider, IDictionary<string, object?>? metadata)
        : this(
            serviceProvider.GetRequiredService<T>,
            ServiceHelper.GetServiceMetadata<TMetadata>(metadata))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenGenericFactoryService{T,TImplementation,TMetadata}"/> class.
    /// </summary>
    /// <param name="factory">The service factory.</param>
    /// <param name="metadata">The metadata.</param>
    protected internal OpenGenericFactoryService(Func<T> factory, TMetadata metadata)
        : base(factory)
    {
        Metadata = metadata;
    }

    /// <summary>
    /// Gets the metadata associated with the export.
    /// </summary>
    /// <value>
    /// The metadata associated with the export.
    /// </value>
    public TMetadata Metadata { get; }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>
    /// A string that represents the current object.
    /// </returns>
    public override string ToString() =>
        $"{GetType().Name} of {typeof(T).Name}/{typeof(TImplementation).Name} with {typeof(TMetadata).Name}.";
}
