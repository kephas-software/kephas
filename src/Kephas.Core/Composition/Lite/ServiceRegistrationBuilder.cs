// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRegistrationBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service registration builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Lite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Composition.Lite.Internal;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Resources;
    using Kephas.Services;

    /// <summary>
    /// A service registration builder.
    /// </summary>
    internal class ServiceRegistrationBuilder : IServiceRegistrationBuilder
    {
        private readonly IAmbientServices ambientServices;

        private Type contractType;

        private Type serviceType;

        private AppServiceLifetime lifetime = AppServiceLifetime.Singleton;

        private bool allowMultiple = false;

        private object instancing;

        private IDictionary<string, object> metadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRegistrationBuilder"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceType">Type of the service.</param>
        public ServiceRegistrationBuilder(IAmbientServices ambientServices, Type serviceType)
        {
            this.serviceType = serviceType;
            this.contractType = serviceType;
            this.ambientServices = ambientServices;
        }

        /// <summary>
        /// Builds the configured information into a <see cref="IServiceInfo"/> and returns it.
        /// </summary>
        /// <returns>
        /// An <see cref="IServiceInfo"/>.
        /// </returns>
        public IServiceInfo Build()
        {
            switch (this.instancing)
            {
                case Type implementationType:
                    this.EnsureContractTypeMatchesImplementationType(this.contractType, implementationType);
                    return new ServiceInfo(this.ambientServices, this.contractType, implementationType, this.lifetime == AppServiceLifetime.Singleton)
                    {
                        AllowMultiple = this.allowMultiple,
                        ServiceType = this.serviceType,
                    };
                case Func<ICompositionContext, object> factory:
                    return new ServiceInfo(this.ambientServices, this.contractType, factory, this.lifetime == AppServiceLifetime.Singleton)
                    {
                        AllowMultiple = this.allowMultiple,
                        ServiceType = this.serviceType,
                    };
                default:
                    if (this.instancing == null)
                    {
                        throw new InvalidOperationException(Strings.ServiceRegistrationBuilder_InstancingNotProvided_Exception);
                    }

                    return new ServiceInfo(this.contractType, this.instancing)
                    {
                        AllowMultiple = this.allowMultiple,
                        ServiceType = this.serviceType,
                    };
            }
        }

        /// <summary>
        /// Sets the registration key to a super type of the service type.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        public IServiceRegistrationBuilder Keyed(Type contractType)
        {
            Requires.NotNull(contractType, nameof(contractType));

            if (!this.contractType.IsAssignableFrom(this.serviceType))
            {
                throw new InvalidOperationException(
                    string.Format(
                        Strings.AmbientServices_ServiceTypeMustBeSuperOfContractType_Exception,
                        this.serviceType,
                        contractType));
            }

            this.contractType = contractType;

            return this;
        }

        /// <summary>
        /// Registers the service as a singleton.
        /// </summary>
        /// <returns>
        /// This builder.
        /// </returns>
        public IServiceRegistrationBuilder AsSingleton()
        {
            this.lifetime = AppServiceLifetime.Singleton;
            return this;
        }

        /// <summary>
        /// Registers the service as transient.
        /// </summary>
        /// <returns>
        /// This builder.
        /// </returns>
        public IServiceRegistrationBuilder AsTransient()
        {
            this.lifetime = AppServiceLifetime.Transient;
            return this;
        }

        /// <summary>
        /// Registers the service with multiple instances.
        /// </summary>
        /// <returns>
        /// This builder.
        /// </returns>
        public IServiceRegistrationBuilder AllowMultiple()
        {
            this.allowMultiple = true;
            return this;
        }

        /// <summary>
        /// Registers the service with the provided instance.
        /// </summary>
        /// <param name="instance">The service instance.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        public IServiceRegistrationBuilder WithInstance(object instance)
        {
            Requires.NotNull(instance, nameof(instance));

            if (!this.contractType.IsInstanceOfType(instance))
            {
                throw new InvalidOperationException(
                    string.Format(
                        Strings.AmbientServices_ServiceTypeAndServiceInstanceMismatch_Exception,
                        instance.GetType(),
                        this.contractType));
            }

            this.instancing = instance;
            return this;
        }

        /// <summary>
        /// Registers the service with the provided factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        public IServiceRegistrationBuilder WithFactory(Func<ICompositionContext, object> factory)
        {
            Requires.NotNull(factory, nameof(factory));

            this.instancing = factory;
            return this;
        }

        /// <summary>
        /// Registers the service with the provided implementation type.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="implementationType">The implementation type.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        public IServiceRegistrationBuilder WithType(Type implementationType)
        {
            Requires.NotNull(implementationType, nameof(implementationType));

            if (!this.serviceType.IsAssignableFrom(implementationType)
                && !(this.serviceType.IsGenericTypeDefinition
                     && !implementationType.IsGenericTypeDefinition
                     && implementationType.GetInterfaces().Any(i => i.IsGenericType && ReferenceEquals(i.GetGenericTypeDefinition(), this.serviceType)))
                && !(this.serviceType.IsGenericTypeDefinition
                     && implementationType.IsGenericTypeDefinition
                     && implementationType.GetInterfaces().Any(i => i.Name == this.serviceType.Name)))
            {
                throw new ArgumentException(
                    string.Format(
                        Strings.AmbientServices_ServiceTypeAndImplementationMismatch_Exception,
                        implementationType,
                        this.contractType),
                    nameof(implementationType));
            }

            this.instancing = implementationType;
            return this;
        }

        private void EnsureContractTypeMatchesImplementationType(Type contractType, Type implementationType)
        {
            if (contractType.IsGenericTypeDefinition && !implementationType.IsGenericTypeDefinition)
            {
                throw new ArgumentException(
                    string.Format("The implementation type {0} must be also a generic type definition for contract {1}.", implementationType, contractType),
                    nameof(implementationType));
            }

            if (!this.contractType.IsGenericTypeDefinition && implementationType.IsGenericTypeDefinition)
            {
                throw new ArgumentException(
                    string.Format("The implementation type {0} must not be a generic type definition for contract {1}.", implementationType, contractType),
                    nameof(implementationType));
            }
        }

        /// <summary>
        /// Adds metadata in form of (key, value) pairs.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        public IServiceRegistrationBuilder AddMetadata(string key, object value)
        {
            if (this.metadata == null)
            {
                this.metadata = new Dictionary<string, object> { { key, value } };
            }
            else
            {
                this.metadata[key] = value;
            }

            return this;
        }
    }
}