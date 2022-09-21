// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceInfoBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Builder;

using System.Reflection;
using Kephas.Services;
using Kephas.Services.Reflection;

/// <summary>
/// Builder class for an <see cref="AppServiceInfo"/>.
/// </summary>
public class AppServiceInfoBuilder : IRegistrationBuilder
{
    private readonly Type contractDeclarationType;
    private readonly object instancingStrategy;
    private IDictionary<string, object?>? metadata;
    private Type contractType;
    private AppServiceLifetime lifetime = AppServiceLifetime.Transient;
    private bool allowMultiple = false;
    private bool isExternallyOwned = false;
    private Func<IEnumerable<ConstructorInfo>, ConstructorInfo?>? constructorSelector;
    private Action<ParameterInfo, IParameterBuilder>? parameterBuilder;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppServiceInfoBuilder"/> class.
    /// </summary>
    /// <param name="contractDeclarationType">The contract declaration type.</param>
    /// <param name="instancingStrategy">The instancing strategy.</param>
    public AppServiceInfoBuilder(Type contractDeclarationType, object instancingStrategy)
    {
        this.contractType = this.contractDeclarationType = contractDeclarationType ?? throw new ArgumentNullException(nameof(contractDeclarationType));
        this.instancingStrategy = instancingStrategy ?? throw new ArgumentNullException(nameof(instancingStrategy));
    }

    /// <summary>
    /// Sets the registration contract.
    /// </summary>
    /// <remarks>
    /// The registration contract is the key to find the service.
    /// The registered service type is a subtype providing additional information, typically metadata.
    /// </remarks>
    /// <param name="contractType">Type of the contract.</param>
    /// <returns>
    /// A registration builder allowing further configuration.
    /// </returns>
    public IRegistrationBuilder As(Type contractType)
    {
        this.contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));

        return this;
    }

    /// <summary>
    /// Registers the service as a singleton.
    /// </summary>
    /// <returns>
    /// A registration builder allowing further configuration.
    /// </returns>
    public IRegistrationBuilder Singleton()
    {
        this.lifetime = AppServiceLifetime.Singleton;

        return this;
    }

    /// <summary>
    /// Registers the service as scoped.
    /// </summary>
    /// <returns>
    /// A registration builder allowing further configuration.
    /// </returns>
    public IRegistrationBuilder Scoped()
    {
        this.lifetime = AppServiceLifetime.Scoped;

        return this;
    }

    /// <summary>
    /// Registers the service with multiple instances.
    /// </summary>
    /// <param name="value">Optional. True if multiple service registrations are allowed (default), false otherwise.</param>
    /// <returns>
    /// A registration builder allowing further configuration.
    /// </returns>
    public IRegistrationBuilder AllowMultiple(bool value = true)
    {
        this.allowMultiple = value;

        return this;
    }

    /// <summary>
    /// Select which of the available constructors will be used to instantiate the part.
    /// </summary>
    /// <param name="constructorSelector">Filter that selects a single constructor.</param>
    /// <param name="parameterBuilder">The parameter builder.</param>
    /// <returns>
    /// A registration builder allowing further configuration.
    /// </returns>
    public IRegistrationBuilder SelectConstructor(Func<IEnumerable<ConstructorInfo>, ConstructorInfo?> constructorSelector, Action<ParameterInfo, IParameterBuilder>? parameterBuilder = null)
    {
        this.constructorSelector = constructorSelector ?? throw new ArgumentNullException(nameof(constructorSelector));
        this.parameterBuilder = parameterBuilder;

        return this;
    }

    /// <summary>
    /// Adds metadata to the export.
    /// </summary>
    /// <param name="name">The name of the metadata item.</param>
    /// <param name="value">The metadata value.</param>
    /// <returns>
    /// A registration builder allowing further configuration.
    /// </returns>
    public IRegistrationBuilder AddMetadata(string name, object? value)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        (this.metadata ??= new Dictionary<string, object?>())[name] = value;

        return this;
    }

    /// <summary>
    /// Indicates whether the created instances are disposed by an external owner.
    /// </summary>
    /// <returns>
    /// This builder.
    /// </returns>
    public IRegistrationBuilder ExternallyOwned()
    {
        this.isExternallyOwned = true;

        return this;
    }

    /// <summary>
    /// Builds the <see cref="IAppServiceInfo"/> instance.
    /// </summary>
    /// <returns>The <see cref="IAppServiceInfo"/> instance.</returns>
    public IAppServiceInfo Build()
    {
        var appServiceInfo = new AppServiceInfo(this.contractType, this.instancingStrategy, this.lifetime, this.contractType.IsGenericTypeDefinition, this.metadata)
            {
                AllowMultiple = this.allowMultiple,
                IsExternallyOwned = this.isExternallyOwned,
            };

        return appServiceInfo;
    }
}