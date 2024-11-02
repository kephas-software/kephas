// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application service information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.DependencyInjection.Reflection;

/// <summary>
/// Information about the application service.
/// </summary>
public record ServiceInfo : IServiceInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceInfo"/> class.
    /// </summary>
    /// <param name="serviceType">The type used to register and retrieve the service. Typically, this is an interface.</param>
    /// <param name="lifetime">Optional. The application service lifetime.</param>
    /// <param name="asOpenGeneric">Optional.
    ///                             <c>true</c> if the contract should be exported as an open generic;
    ///                             otherwise, <c>false</c>.
    /// </param>
    public ServiceInfo(Type serviceType, AppServiceLifetime lifetime = AppServiceLifetime.Transient, bool? asOpenGeneric = null)
    {
        _ = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

        SetServiceType(serviceType);
        AsOpenGeneric = asOpenGeneric;
        SetLifetime(lifetime);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceInfo"/> class.
    /// </summary>
    /// <param name="serviceType">The type used to register and retrieve the service. Typically, this is an interface.</param>
    /// <param name="instance">The service instance.</param>
    public ServiceInfo(Type serviceType, object instance)
    {
        _ = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        _ = instance ?? throw new ArgumentNullException(nameof(instance));

        SetServiceType(serviceType);
        InstancingStrategy = instance;
        SetLifetime(AppServiceLifetime.Singleton);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceInfo"/> class.
    /// </summary>
    /// <param name="serviceType">The type used to register and retrieve the service. Typically, this is an interface.</param>
    /// <param name="factory">The service instance factory.</param>
    /// <param name="lifetime">Optional. The application service lifetime.</param>
    public ServiceInfo(Type serviceType, Func<IServiceProvider, object> factory, AppServiceLifetime lifetime = AppServiceLifetime.Transient)
    {
        _ = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        _ = factory ?? throw new ArgumentNullException(nameof(factory));

        SetServiceType(serviceType);
        InstancingStrategy = factory;
        SetLifetime(lifetime);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceInfo"/> class.
    /// </summary>
    /// <param name="serviceType">The type used to register and retrieve the service. Typically, this is an interface.</param>
    /// <param name="implementationType">The service implementation type.</param>
    /// <param name="lifetime">Optional. The application service lifetime.</param>
    /// <param name="asOpenGeneric">Optional.
    ///                             <c>true</c> if the contract should be exported as an open generic;
    ///                             otherwise, <c>false</c>.
    /// </param>
    public ServiceInfo(Type serviceType, Type implementationType, AppServiceLifetime lifetime = AppServiceLifetime.Transient, bool asOpenGeneric = false)
    {
        serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        implementationType = implementationType ?? throw new ArgumentNullException(nameof(implementationType));

        SetServiceType(serviceType);
        InstancingStrategy = implementationType;
        AsOpenGeneric = asOpenGeneric;
        SetLifetime(lifetime);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceInfo"/> class.
    /// </summary>
    /// <param name="serviceInfo">The <see cref="IServiceInfo"/> from which the information should be copied.</param>
    /// <param name="serviceType">The contract type, in case the service information does not have it already.</param>
    /// <param name="serviceDeclarationType">The type declaring the service type.</param>
    /// <param name="instancingStrategy">Optional. The instancing strategy.</param>
    internal ServiceInfo(IServiceInfo serviceInfo, Type? serviceType, Type? serviceDeclarationType, object? instancingStrategy = null)
    {
        _ = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        _ = serviceInfo ?? throw new ArgumentNullException(nameof(serviceInfo));

        SetServiceType(serviceType ?? serviceInfo.ServiceType);
        if (serviceDeclarationType != null)
        {
            ServiceDeclarationType = serviceDeclarationType;
        }

        InstancingStrategy = instancingStrategy ?? serviceInfo.InstancingStrategy;

        AsOpenGeneric = serviceInfo.AsOpenGeneric;
        SetLifetime(serviceInfo.Lifetime);
        Metadata = serviceInfo.Metadata;
        MetadataType = serviceInfo.MetadataType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceInfo"/> class.
    /// </summary>
    /// <param name="serviceType">The type used to register and retrieve the service. Typically, this is an interface.</param>
    /// <param name="serviceDeclarationType">The service declaration type.</param>
    /// <param name="instancingStrategy">Type of the service instance.</param>
    /// <param name="lifetime">Optional. The application service lifetime.</param>
    /// <param name="asOpenGeneric">Optional.
    ///                             <c>true</c> if the contract should be exported as an open generic;
    ///                             otherwise, <c>false</c>.
    /// </param>
    /// <param name="metadata">The metadata.</param>
    internal ServiceInfo(Type serviceType, Type? serviceDeclarationType, object? instancingStrategy, AppServiceLifetime lifetime, bool asOpenGeneric, IDictionary<string, object?>? metadata)
    {
        serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        instancingStrategy = instancingStrategy ?? throw new ArgumentNullException(nameof(instancingStrategy));

        SetServiceType(serviceType);
        ServiceDeclarationType = serviceDeclarationType ?? serviceType;
        InstancingStrategy = instancingStrategy;
        AsOpenGeneric = asOpenGeneric;
        SetLifetime(lifetime);
        Metadata = metadata;
    }

    /// <summary>
    /// Gets the application service lifetime.
    /// </summary>
    /// <value>
    /// The application service lifetime.
    /// </value>
    public AppServiceLifetime Lifetime { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether the contract should be exported as an open generic.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the contract should be exported as an open generic; otherwise, <c>false</c>.
    /// </value>
    public bool? AsOpenGeneric { get; private set; }

    /// <summary>
    /// Gets the metadata attached to the service information.
    /// </summary>
    public IDictionary<string, object?>? Metadata { get; private set; }

    /// <summary>
    /// Gets The type used to register and retrieve the service. Typically, this is an interface.
    /// </summary>
    /// <value>
    /// The type used to register and retrieve the service. Typically, this is an interface.
    /// </value>
    public Type? ServiceType { get; private set; }

    /// <summary>
    /// Gets the type declaring the service type.
    /// </summary>
    /// <remarks>
    /// This is the type annotated with one of the: [<see cref="TransientServiceAttribute"/>], [<see cref="ScopedServiceAttribute"/>],
    /// or [<see cref="SingletonServiceAttribute"/>], and which declares the actual service type.
    /// Typically, this is a generic type used for collecting metadata, redirecting to the non-generic service type.
    /// </remarks>
    public Type? ServiceDeclarationType { get; private set; }

    /// <summary>
    /// Gets or sets the supported metadata type.
    /// </summary>
    /// <value>
    /// The supported metadata type.
    /// </value>
    public Type? MetadataType { get; set; }

    /// <summary>
    /// Gets the instancing strategy: factory, type, or instance.
    /// </summary>
    public object? InstancingStrategy { get; }

    /// <summary>
    /// Adds the metadata with the provided name and value.
    /// </summary>
    /// <param name="name">The metadata name.</param>
    /// <param name="value">The metadata value.</param>
    /// <returns>This <see cref="ServiceInfo"/>.</returns>
    public IServiceInfo AddMetadata(string name, object? value)
    {
        ((Metadata ??= new Dictionary<string, object?>())!)[name] = value;
        return this;
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>
    /// A string that represents the current object.
    /// </returns>
    public override string ToString()
    {
        var openGeneric = 
            AsOpenGeneric is null
                ? string.Empty :
                AsOpenGeneric is true
                    ? ", open generic"
                    : ", closed generic";
        IServiceInfo serviceInfo = this;
        var instanceTypeString = serviceInfo.ImplementationType != null ? $"/type:{serviceInfo.ImplementationType}" : string.Empty;
        var instanceString = serviceInfo.Instance != null ? $"/instance:{serviceInfo.Instance}" : string.Empty;
        var factoryString = serviceInfo.Factory != null ? "/instanceFactory" : string.Empty;

        return $"{ServiceType}{openGeneric}, {Lifetime}{instanceTypeString}{instanceString}{factoryString}";
    }

    private void SetServiceType(Type? serviceType)
    {
        ServiceType = serviceType;
        ServiceDeclarationType = serviceType;
        if (serviceType?.IsGenericTypeDefinition ?? false)
        {
            AsOpenGeneric = true;
        }
    }

    private void SetLifetime(AppServiceLifetime lifetime)
    {
        Lifetime = lifetime;
    }
}
