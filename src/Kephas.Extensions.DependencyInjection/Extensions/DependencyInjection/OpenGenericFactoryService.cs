// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpenGenericFactoryService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection;

using Kephas.Services;
using Kephas.Services;

/// <summary>
/// Service returning a <see cref="T"/> service based on the <see cref="TImplementation"/>.
/// </summary>
/// <typeparam name="T">The service contract type.</typeparam>
public class OpenGenericFactoryService<T> : IExportFactory<T>
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
    /// Creates the exported value.
    /// </summary>
    /// <returns>
    /// The exported value.
    /// </returns>
    public T CreateExportedValue() => this.factory();
}

/// <summary>
/// Service returning a <see cref="T"/> service based on the <see cref="TImplementation"/> with associated <see cref="TMetadata"/>.
/// </summary>
/// <typeparam name="T">The service contract type.</typeparam>
/// <typeparam name="TImplementation">The service implementation type.</typeparam>
/// <typeparam name="TMetadata">The metadata type.</typeparam>
public class OpenGenericFactoryService<T, TImplementation, TMetadata> : OpenGenericFactoryService<T>, IExportFactory<T, TMetadata>
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
        this.Metadata = metadata;
    }

    /// <summary>
    /// Gets the metadata associated with the export.
    /// </summary>
    /// <value>
    /// The metadata associated with the export.
    /// </value>
    public TMetadata Metadata { get; }
}