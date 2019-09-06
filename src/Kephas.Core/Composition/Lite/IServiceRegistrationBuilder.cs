// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceRegistrationBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IServiceRegistrationBuilder interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Lite
{
    using Kephas.Composition;
    using System;

    /// <summary>
    /// Interface for service registration builder.
    /// </summary>
    public interface IServiceRegistrationBuilder
    {
        /// <summary>
        /// Sets the registration type to a super type of the service type.
        /// </summary>
        /// <remarks>
        /// The registration type is the key to find the service.
        /// The registered service type is a subtype providing additional information, typically metadata.
        /// </remarks>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        IServiceRegistrationBuilder RegisterAs(Type contractType);

        /// <summary>
        /// Registers the service as a singleton.
        /// </summary>
        /// <returns>
        /// This builder.
        /// </returns>
        IServiceRegistrationBuilder AsSingleton();

        /// <summary>
        /// Registers the service as transient.
        /// </summary>
        /// <returns>
        /// This builder.
        /// </returns>
        IServiceRegistrationBuilder AsTransient();

        /// <summary>
        /// Registers the service with multiple instances.
        /// </summary>
        /// <returns>
        /// This builder.
        /// </returns>
        IServiceRegistrationBuilder AllowMultiple();

        /// <summary>
        /// Registers the service with the provided instance.
        /// </summary>
        /// <param name="instance">The service instance.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        IServiceRegistrationBuilder WithInstance(object instance);

        /// <summary>
        /// Registers the service with the provided factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        IServiceRegistrationBuilder WithFactory(Func<ICompositionContext, object> factory);

        /// <summary>
        /// Registers the service with the provided implementation type.
        /// </summary>
        /// <param name="implementationType">The implementation type.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        IServiceRegistrationBuilder WithType(Type implementationType);

        /// <summary>
        /// Adds metadata in form of (key, value) pairs.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        IServiceRegistrationBuilder AddMetadata(string key, object value);
    }
}