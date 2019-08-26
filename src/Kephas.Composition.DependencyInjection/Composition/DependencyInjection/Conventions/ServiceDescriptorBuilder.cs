// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceDescriptorBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service descriptor builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.DependencyInjection.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Composition.Conventions;
    using Kephas.Logging;

    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A service descriptor builder.
    /// </summary>
    internal class ServiceDescriptorBuilder : Loggable, IExportConventionsBuilder
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
        /// Gets or sets the implementation type predicate.
        /// </summary>
        /// <value>
        /// The implementation type predicate.
        /// </value>
        public Predicate<Type> ImplementationTypePredicate { get; set; }

        /// <summary>
        /// Gets or sets the lifetime.
        /// </summary>
        /// <value>
        /// The lifetime.
        /// </value>
        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;

        /// <summary>
        /// Gets or sets the export configuration.
        /// </summary>
        /// <value>
        /// The export configuration.
        /// </value>
        public Action<Type, IExportConventionsBuilder> ExportConfiguration { get; set; }

        /// <summary>
        /// Builds the information into a service descriptor.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="parts">The parts.</param>
        /// <returns>
        /// A ServiceDescriptor.
        /// </returns>
        public IEnumerable<ServiceDescriptor> Build(IEnumerable<Type> parts)
        {
            var descriptor = this.Instance != null
                                 ? new ServiceDescriptor(this.ServiceType ?? this.Instance.GetType(), this.Instance)
                                 : this.ImplementationType != null
                                     ? new ServiceDescriptor(this.ServiceType ?? this.ImplementationType, this.ImplementationType, this.Lifetime)
                                     : this.Factory != null
                                         ? new ServiceDescriptor(this.ServiceType, this.Factory, this.Lifetime)
                                         : null;

            if (descriptor != null)
            {
                this.ExportConfiguration?.Invoke(this.ImplementationType, this);
                yield return descriptor;
                yield break;
            }

            if (this.ImplementationTypePredicate != null)
            {
                foreach (var type in parts.Where(t => this.ImplementationTypePredicate(t)))
                {
                    this.ExportConfiguration?.Invoke(type, this);
                    yield return new ServiceDescriptor(this.ServiceType ?? type, type, this.Lifetime);
                }

                yield break;
            }

            throw new InvalidOperationException(
                $"One of {nameof(this.Instance)}, {nameof(this.ImplementationType)}, {nameof(this.ImplementationTypePredicate)}, or {nameof(this.Factory)} must be set.");
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
                                       ?? (this.ImplementationTypePredicate != null ? "type predicate" :
                                           this.Factory != null ? "factory" :
                                           this.Instance != null ? "instance" : "unknown");
            return $"{this.ServiceType}/{this.Lifetime}/{implementationString}";
        }

        /// <summary>
        /// Specify the contract type for the export.
        /// </summary>
        /// <param name="contractType">The contract type.</param>
        /// <returns>
        /// An export builder allowing further configuration.
        /// </returns>
        public IExportConventionsBuilder AsContractType(Type contractType)
        {
            this.ServiceType = contractType;
            return this;
        }

        /// <summary>
        /// Add export metadata to the export.
        /// </summary>
        /// <param name="name">The name of the metadata item.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>
        /// An export builder allowing further configuration.
        /// </returns>
        public IExportConventionsBuilder AddMetadata(string name, object value)
        {
            this.Logger.Warn($"Metadata not supported. Service type: {this.ServiceType}.");
            return this;
        }

        /// <summary>
        /// Add export metadata to the export.
        /// </summary>
        /// <param name="name">The name of the metadata item.</param>
        /// <param name="getValueFromPartType">A function that calculates the metadata value based on
        ///                                    the type.</param>
        /// <returns>
        /// An export builder allowing further configuration.
        /// </returns>
        public IExportConventionsBuilder AddMetadata(string name, Func<Type, object> getValueFromPartType)
        {
            this.Logger.Warn($"Metadata not supported. Service type: {this.ServiceType}.");
            return this;
        }
    }
}