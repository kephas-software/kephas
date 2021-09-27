// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceRegistrationBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IServiceRegistrationBuilder interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Lite
{
    using System;

    /// <summary>
    /// Interface for service registration builder.
    /// </summary>
    public interface IServiceRegistrationBuilder
    {
        /// <summary>
        /// Sets the registration contract to a super type of the service type.
        /// </summary>
        /// <remarks>
        /// The registration contract is the key to find the service.
        /// The registered service type is a subtype providing additional information, typically metadata.
        /// </remarks>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        IServiceRegistrationBuilder As(Type contractType);

        /// <summary>
        /// Registers the service as a singleton.
        /// </summary>
        /// <returns>
        /// This builder.
        /// </returns>
        IServiceRegistrationBuilder Singleton();

        /// <summary>
        /// Registers the service as scoped.
        /// </summary>
        /// <returns>
        /// This builder.
        /// </returns>
        IServiceRegistrationBuilder Scoped();

        /// <summary>
        /// Registers the service as transient.
        /// </summary>
        /// <returns>
        /// This builder.
        /// </returns>
        IServiceRegistrationBuilder Transient();

        /// <summary>
        /// Registers the service with multiple instances.
        /// </summary>
        /// <returns>
        /// This builder.
        /// </returns>
        IServiceRegistrationBuilder AllowMultiple();

        /// <summary>
        /// Registers the service with the provided instancing strategy.
        /// </summary>
        /// <param name="instancingStrategy">The service instancing strategy.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        IServiceRegistrationBuilder WithInstancingStrategy(object instancingStrategy);

        /// <summary>
        /// Registers the service with the provided instance.
        /// </summary>
        /// <param name="instance">The service instance.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        IServiceRegistrationBuilder WithInstance(object instance)
            => this.WithInstancingStrategy(instance);

        /// <summary>
        /// Registers the service with the provided factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        IServiceRegistrationBuilder WithFactory(Func<IInjector, object> factory)
            => this.WithInstancingStrategy(factory);

        /// <summary>
        /// Registers the service with the provided implementation type.
        /// </summary>
        /// <param name="implementationType">The implementation type.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        IServiceRegistrationBuilder WithType(Type implementationType)
            => this.WithInstancingStrategy(implementationType);

        /// <summary>
        /// Adds metadata in form of (key, value) pairs.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        IServiceRegistrationBuilder AddMetadata(string key, object? value);

        /// <summary>
        /// Indicates whether the created instances are disposed by an external owner.
        /// </summary>
        /// <param name="value">True if externally owned, false otherwise.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        IServiceRegistrationBuilder ExternallyOwned(bool value);
    }
}