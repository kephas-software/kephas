// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceRegistrationBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IServiceRegistrationBuilder interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Lite.Builder
{
    using System;
    using Kephas.Injection.Builder;

    /// <summary>
    /// Interface for service registration builder.
    /// </summary>
    public interface IServiceRegistrationBuilder : IRegistrationBuilder
    {
        /// <summary>
        /// Registers the service as transient.
        /// </summary>
        /// <returns>
        /// This builder.
        /// </returns>
        IServiceRegistrationBuilder Transient();

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
        /// Indicates whether the created instances are disposed by an external owner.
        /// </summary>
        /// <param name="value">True if externally owned, false otherwise.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        IServiceRegistrationBuilder ExternallyOwned(bool value);
    }
}