// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceRegistrationBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IServiceRegistrationBuilder interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Lightweight
{
    using System;

    /// <summary>
    /// Interface for service registration builder.
    /// </summary>
    public interface IServiceRegistrationBuilder
    {
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
    }
}