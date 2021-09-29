// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceDescriptorBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service descriptor builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection.Conventions
{
    using System;
    using System.Collections.Generic;

    using Kephas.Injection;
    using Kephas.Logging;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A service descriptor builder.
    /// </summary>
    internal class ServiceDescriptorBuilder : Loggable
    {
        /// <summary>
        /// Gets or sets the type of the service.
        /// </summary>
        /// <value>
        /// The type of the service.
        /// </value>
        public Type? ContractType { get; set; }

        /// <summary>
        /// Gets or sets the instancing strategy.
        /// </summary>
        public object? InstancingStrategy { get; set; }

        /// <summary>
        /// Gets the service instance.
        /// </summary>
        /// <value>
        /// The service instance.
        /// </value>
        public object? Instance => this.Factory == null && this.ImplementationType == null ? this.InstancingStrategy : null;

        /// <summary>
        /// Gets the factory.
        /// </summary>
        /// <value>
        /// A function delegate that yields an object.
        /// </value>
        public Func<IServiceProvider, object>? Factory => this.InstancingStrategy as Func<IServiceProvider, object>;

        /// <summary>
        /// Gets the type of the implementation.
        /// </summary>
        /// <value>
        /// The type of the implementation.
        /// </value>
        public Type? ImplementationType => this.InstancingStrategy as Type;

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
        public IEnumerable<ServiceDescriptor> Build()
        {
            var descriptor = this.ImplementationType != null
                                     ? new ServiceDescriptor(this.ContractType ?? this.ImplementationType, this.ImplementationType, this.Lifetime)
                                     : this.Factory != null
                                         ? new ServiceDescriptor(this.ContractType, this.Factory, this.Lifetime)
                                         : this.Instance != null
                                             ? new ServiceDescriptor(this.ContractType ?? this.Instance.GetType(), this.Instance)
                                             : null;

            if (descriptor != null)
            {
                yield return descriptor;
                yield break;
            }

            throw new InjectionException(
                $"One of {nameof(this.Instance)}, {nameof(this.ImplementationType)}, or {nameof(this.Factory)} must be set.");
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var implementationString = this.ImplementationType?.ToString()
                                       ?? (this.Factory != null ? "factory" :
                                           this.Instance != null ? "instance" : "unknown");
            return $"{this.ContractType}/{this.Lifetime}/{implementationString}";
        }

        /// <summary>
        /// Add export metadata to the export.
        /// </summary>
        /// <param name="name">The name of the metadata item.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>
        /// An export builder allowing further configuration.
        /// </returns>
        public ServiceDescriptorBuilder AddMetadata(string name, object? value)
        {
            this.Logger.Warn("Metadata not supported. Service type: {serviceType}.", this.ContractType);
            return this;
        }
    }
}