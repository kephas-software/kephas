// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceDescriptorBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service descriptor builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Medi.Conventions
{
    using System;

    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A service descriptor builder.
    /// </summary>
    internal class ServiceDescriptorBuilder
    {
        /// <summary>
        /// Gets or sets the type of the service.
        /// </summary>
        /// <value>
        /// The type of the service.
        /// </value>
        public Type ServiceType { get; set; }

        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public object Instance { get; set; }

        /// <summary>
        /// Gets or sets the factory.
        /// </summary>
        /// <value>
        /// A function delegate that yields an object.
        /// </value>
        public Func<IServiceProvider, object> Factory { get; set; }

        /// <summary>
        /// Gets or sets the type of the implementation.
        /// </summary>
        /// <value>
        /// The type of the implementation.
        /// </value>
        public Type ImplementationType { get; set; }

        /// <summary>
        /// Gets or sets the lifetime.
        /// </summary>
        /// <value>
        /// The lifetime.
        /// </value>
        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;

        /// <summary>
        /// Builds the information into a service descriptor.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <returns>
        /// A ServiceDescriptor.
        /// </returns>
        public ServiceDescriptor Build()
        {
            return this.Instance != null
                       ? new ServiceDescriptor(this.ServiceType, this.Instance)
                       : this.ImplementationType != null
                            ? new ServiceDescriptor(this.ServiceType, this.ImplementationType, this.Lifetime)
                            : this.Factory != null
                                ? new ServiceDescriptor(this.ServiceType, this.Factory, this.Lifetime)
                                : throw new InvalidOperationException($"One of Instance, ImplementationType, or Factory must be set.");
        }
    }
}