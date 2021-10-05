// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRegistrationBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service registration builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Lite.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Injection.Builder;
    using Kephas.Injection.Lite.Internal;
    using Kephas.Resources;
    using Kephas.Services;

    /// <summary>
    /// A service registration builder.
    /// </summary>
    internal class ServiceRegistrationBuilder : IServiceRegistrationBuilder
    {
        private readonly IAmbientServices ambientServices;

        private readonly Type contractDeclarationType;

        private Type contractType;

        private AppServiceLifetime lifetime = AppServiceLifetime.Singleton;

        private bool allowMultiple = false;

        private bool externallyOwned = true;

        private object instancingStrategy;

        private IDictionary<string, object?>? metadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRegistrationBuilder"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractDeclarationType">The contract declaration type.</param>
        public ServiceRegistrationBuilder(IAmbientServices ambientServices, Type contractDeclarationType)
        {
            this.contractType = this.contractDeclarationType = contractDeclarationType;
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
            switch (this.instancingStrategy)
            {
                case Type implementationType:
                    this.EnsureContractTypeMatchesImplementationType(this.contractType, implementationType);
                    return new ServiceInfo(this.ambientServices, this.contractType, implementationType, this.lifetime != AppServiceLifetime.Transient)
                    {
                        AllowMultiple = this.allowMultiple,
                        ExternallyOwned = this.externallyOwned,
                        Metadata = this.metadata,
                    };
                case Func<IInjector, object> factory:
                    return new ServiceInfo(this.ambientServices, this.contractType, factory, this.lifetime != AppServiceLifetime.Transient)
                    {
                        AllowMultiple = this.allowMultiple,
                        ExternallyOwned = this.externallyOwned,
                        Metadata = this.metadata,
                    };
                default:
                    if (this.instancingStrategy == null)
                    {
                        if (!this.allowMultiple)
                        {
                            throw new InvalidOperationException(Strings.ServiceRegistrationBuilder_InstancingNotProvided_Exception.FormatWith(this.contractType, nameof(AppServiceContractAttribute.AllowMultiple), true));
                        }

                        return new MultiServiceInfo(this.contractType);
                    }

                    return new ServiceInfo(this.ambientServices, this.contractType, this.instancingStrategy)
                    {
                        AllowMultiple = this.allowMultiple,
                        ExternallyOwned = this.externallyOwned,
                        Metadata = this.metadata,
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
        public IRegistrationBuilder As(Type contractType)
        {
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));

            if (!contractType.IsAssignableFrom(this.contractDeclarationType))
            {
                throw new InvalidOperationException(
                    string.Format(
                        Strings.AmbientServices_ServiceTypeMustBeSuperOfContractType_Exception,
                        this.contractType,
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
        public IRegistrationBuilder Singleton()
        {
            this.lifetime = AppServiceLifetime.Singleton;
            return this;
        }

        /// <summary>
        /// Registers the service as a scoped.
        /// </summary>
        /// <returns>
        /// This builder.
        /// </returns>
        public IRegistrationBuilder Scoped()
        {
            this.lifetime = AppServiceLifetime.Scoped;
            return this;
        }

        /// <summary>
        /// Registers the service as transient.
        /// </summary>
        /// <returns>
        /// This builder.
        /// </returns>
        public IServiceRegistrationBuilder Transient()
        {
            this.lifetime = AppServiceLifetime.Transient;
            return this;
        }

        /// <summary>
        /// Registers the service with multiple instances.
        /// </summary>
        /// <param name="value">Optional. True if multiple service registrations are allowed (default), false otherwise.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        public IRegistrationBuilder AllowMultiple(bool value = true)
        {
            this.allowMultiple = true;
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
        public IRegistrationBuilder SelectConstructor(
            Func<IEnumerable<ConstructorInfo>, ConstructorInfo?> constructorSelector,
            Action<ParameterInfo, IParameterBuilder>? parameterBuilder = null)
        {
            // TODO: not supported currently
            return this;
        }

        /// <summary>
        /// Registers the service with the provided instance strategy.
        /// </summary>
        /// <param name="instancingStrategy">The service instance strategy.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        public IServiceRegistrationBuilder WithInstancingStrategy(object instancingStrategy)
        {
            if (instancingStrategy == null)
            {
                throw new ArgumentNullException(nameof(instancingStrategy));
            }

            switch (instancingStrategy)
            {
                case Type implementationType:
                    if (!this.contractType.IsAssignableFrom(implementationType)
                        && !(this.contractType.IsGenericTypeDefinition
                             && !implementationType.IsGenericTypeDefinition
                             && implementationType.GetInterfaces().Any(i => i.IsGenericType && ReferenceEquals(i.GetGenericTypeDefinition(), this.contractType)))
                        && !(this.contractType.IsGenericTypeDefinition
                             && implementationType.IsGenericTypeDefinition
                             && implementationType.GetInterfaces().Any(i => i.Name == this.contractType.Name)))
                    {
                        throw new ArgumentException(
                            string.Format(
                                Strings.AmbientServices_ServiceTypeAndImplementationMismatch_Exception,
                                implementationType,
                                this.contractType),
                            nameof(implementationType));
                    }

                    break;
                case Func<IInjector, object> factory:
                    break;
                case var instance:
                    if (!this.contractType.IsInstanceOfType(instance))
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                Strings.AmbientServices_ServiceTypeAndServiceInstanceMismatch_Exception,
                                instance.GetType(),
                                this.contractType));
                    }

                    break;
            }

            this.instancingStrategy = instancingStrategy;

            return this;
        }

        /// <summary>
        /// Adds metadata in form of (key, value) pairs.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        public IRegistrationBuilder AddMetadata(string key, object? value)
        {
            (this.metadata ??= new Dictionary<string, object?>())[key] = value;

            return this;
        }

        /// <summary>
        /// Indicates whether the created instances are disposed by an external owner.
        /// </summary>
        /// <param name="value">True if externally owned, false otherwise.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        public IServiceRegistrationBuilder ExternallyOwned(bool value)
        {
            this.externallyOwned = value;
            return this;
        }

        private void EnsureContractTypeMatchesImplementationType(Type contractType, Type implementationType)
        {
            if (contractType.IsGenericTypeDefinition && !implementationType.IsGenericTypeDefinition)
            {
                throw new ArgumentException(
                    $"The implementation type {implementationType} must be also a generic type definition for contract {contractType}.",
                    nameof(implementationType));
            }

            if (!this.contractType.IsGenericTypeDefinition && implementationType.IsGenericTypeDefinition)
            {
                throw new ArgumentException(
                    $"The implementation type {implementationType} must not be a generic type definition for contract {contractType}.",
                    nameof(implementationType));
            }
        }
    }
}