// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the export factory class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.DependencyInjection;

/// <summary>
/// A service factory.
/// </summary>
/// <typeparam name="TService">Type of the service contract.</typeparam>
public class Factory<TService> : IFactory<TService>
    where TService : notnull
{
    /// <summary>
    /// The factory.
    /// </summary>
    private readonly Func<TService> factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="Factory{T}"/> class.
    /// </summary>
    /// <param name="factory">The factory.</param>
    public Factory(Func<TService> factory)
    {
        this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    /// <summary>
    /// Gets an instance retrieved from the dependency injection container.
    /// </summary>
    public TService GetService() => factory();

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>
    /// A string that represents the current object.
    /// </returns>
    public override string ToString() =>
        $"{GetType().Name} of {typeof(TService).Name}.";
}

/// <summary>
/// A service factory with metadata.
/// </summary>
/// <typeparam name="TService">Type of the service contract.</typeparam>
/// <typeparam name="TMetadata">Type of the metadata.</typeparam>
public class Factory<TService, TMetadata> : Factory<TService>, IFactory<TService, TMetadata>
    where TService : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Factory{T,TMetadata}"/> class.
    /// </summary>
    /// <param name="factory">The factory.</param>
    /// <param name="metadata">The metadata.</param>
    public Factory(Func<TService> factory, IDictionary<string, object?>? metadata)
        : this(factory, ServiceHelper.GetServiceMetadata<TMetadata>(metadata))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Factory{T,TMetadata}"/> class.
    /// </summary>
    /// <param name="factory">The factory.</param>
    /// <param name="metadata">The metadata.</param>
    public Factory(Func<TService> factory, TMetadata metadata)
        : base(factory)
    {
        Metadata = metadata;
    }

    /// <summary>
    /// Gets or sets the metadata associated with the export.
    /// </summary>
    /// <value>
    /// The metadata associated with the export.
    /// </value>
    public TMetadata Metadata { get; set; }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>
    /// A string that represents the current object.
    /// </returns>
    public override string ToString() =>
        $"{GetType().Name} of {typeof(TService).Name} with {Metadata}.";
}