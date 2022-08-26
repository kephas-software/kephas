// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection;

using Kephas.Injection;
using Kephas.Services;

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
    /// Create an instance of the exported part.
    /// </summary>
    /// <returns>A handle allowing the created part to be accessed then released.</returns>
    public IExport<T> CreateExport() => new Export<T>(() => Tuple.Create<T, Action>(this.factory(), () => { }));
}

public class FactoryService<T, TImplementation, TMetadata> : IExportFactory<T, TMetadata>
    where TImplementation : class, T
{
    private readonly Func<T> factory;

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
    {
        this.factory = factory;
        this.Metadata = metadata;
    }

    /// <summary>
    /// Gets the metadata associated with the export.
    /// </summary>
    /// <value>
    /// The metadata associated with the export.
    /// </value>
    public TMetadata Metadata { get; }

    /// <summary>
    /// Create an instance of the exported part.
    /// </summary>
    /// <returns>A handle allowing the created part to be accessed then released.</returns>
    public IExport<T, TMetadata> CreateExport() => new Export<T, TMetadata>(() => Tuple.Create<T, Action>(this.factory(), () => { }), this.Metadata);
}