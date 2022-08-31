// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection;

using Kephas.Injection;
using Kephas.Services;

/// <summary>
/// Service returning a <see cref="T"/> service based on the <see cref="TImplementation"/>.
/// </summary>
/// <typeparam name="T">The service contract type.</typeparam>
/// <typeparam name="TImplementation">The service implementation type.</typeparam>
public class FactoryService<T, TImplementation> : IExportFactory<T>
    where TImplementation : class, T
{
    private readonly Func<T> factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="FactoryService{T, TImplementation}"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public FactoryService(IServiceProvider serviceProvider)
    : this(() => serviceProvider.GetRequiredService<TImplementation>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FactoryService{T, TImplementation}"/> class.
    /// </summary>
    /// <param name="factory">The service factory.</param>
    protected internal FactoryService(Func<T> factory)
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
public class FactoryService<T, TImplementation, TMetadata> : FactoryService<T, TImplementation>, IExportFactory<T, TMetadata>
    where TImplementation : class, T
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FactoryService{T, TImplementation, TMetadata}"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public FactoryService(IServiceProvider serviceProvider)
        : this(() => serviceProvider.GetRequiredService<TImplementation>(), ServiceHelper.GetServiceMetadata<TMetadata>(typeof(TImplementation), typeof(T)))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FactoryService{T, TImplementation, TMetadata}"/> class.
    /// </summary>
    /// <param name="factory">The service factory.</param>
    /// <param name="metadata">The metadata.</param>
    protected internal FactoryService(Func<T> factory, TMetadata metadata)
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