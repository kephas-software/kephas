// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiteRegistrationBuilder.cs" company="Kephas Software SRL">
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
    using Kephas.Injection.Lite.Resources;
    using Kephas.Resources;
    using Kephas.Services;

    /// <summary>
    /// A service registration builder.
    /// </summary>
    internal class LiteRegistrationBuilder : IRegistrationBuilder
    {
        private readonly IAppServiceRegistry serviceRegistry;

        private Type? contractType;

        private AppServiceLifetime lifetime = AppServiceLifetime.Singleton;

        private bool allowMultiple = false;

        private bool externallyOwned = false;

        private object? instancingStrategy;

        private IDictionary<string, object?>? metadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="LiteRegistrationBuilder"/> class.
        /// </summary>
        /// <param name="serviceRegistry">The ambient services.</param>
        /// <param name="instancingStrategy">The instancing strategy.</param>
        public LiteRegistrationBuilder(IAppServiceRegistry serviceRegistry, object instancingStrategy)
        {
            this.serviceRegistry = serviceRegistry ?? throw new ArgumentNullException(nameof(serviceRegistry));
            this.SetInstancingStrategy(instancingStrategy ?? throw new ArgumentNullException(nameof(instancingStrategy)));
        }

        /// <summary>
        /// Builds the configured information into a <see cref="IServiceInfo"/> and returns it.
        /// </summary>
        /// <returns>
        /// An <see cref="IServiceInfo"/>.
        /// </returns>
        public IServiceInfo Build()
        {
            if (this.contractType is null)
            {
                throw new InjectionException(LiteStrings.LiteRegistrationBuilder_Build_ContractTypeNotSet);
            }

            switch (this.instancingStrategy)
            {
                case Type serviceType:
                    this.EnsureContractTypeMatchesImplementationType(this.contractType, serviceType);
                    return new ServiceInfo(this.serviceRegistry, this.contractType, serviceType, this.lifetime != AppServiceLifetime.Transient)
                    {
                        AllowMultiple = this.allowMultiple,
                        IsExternallyOwned = this.externallyOwned,
                        Metadata = this.metadata,
                    };
                case Func<IServiceProvider?, object> factory:
                    return new ServiceInfo(this.serviceRegistry, this.contractType, factory, this.lifetime != AppServiceLifetime.Transient)
                    {
                        AllowMultiple = this.allowMultiple,
                        IsExternallyOwned = this.externallyOwned,
                        Metadata = this.metadata,
                    };
                case { } instance:
                    return new ServiceInfo(this.serviceRegistry, this.contractType, instance)
                    {
                        AllowMultiple = this.allowMultiple,
                        IsExternallyOwned = this.externallyOwned,
                        Metadata = this.metadata,
                    };
                case var _:
                    return this.allowMultiple
                        ? new MultiServiceInfo(this.contractType)
                        : throw new InvalidOperationException(
                            AbstractionStrings.ServiceRegistrationBuilder_InstancingNotProvided_Exception.FormatWith(
                                this.contractType, nameof(AppServiceContractAttribute.AllowMultiple), true));
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
            this.contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
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
        public IRegistrationBuilder Transient()
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
        /// <returns>
        /// This builder.
        /// </returns>
        public IRegistrationBuilder ExternallyOwned()
        {
            this.externallyOwned = true;
            return this;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var implementationString = this.instancingStrategy is Type serviceType
                                       ? serviceType.ToString()
                                       : this.instancingStrategy is Delegate
                                           ? "factory"
                                           : this.instancingStrategy != null
                                               ? "instance"
                                               : "unknown";
            return $"{this.contractType}/{this.lifetime}/{implementationString}";
        }

        /// <summary>
        /// Registers the service with the provided instance strategy.
        /// </summary>
        /// <param name="instancingStrategy">The service instance strategy.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        private IRegistrationBuilder SetInstancingStrategy(object instancingStrategy)
        {
            var inferredContractType = instancingStrategy switch
            {
                Type implementationType =>
                    this.MatchesContractType(implementationType)
                        ? implementationType
                        : throw new ArgumentException(
                            string.Format(
                                AbstractionStrings.AmbientServices_ServiceTypeAndImplementationMismatch_Exception,
                                implementationType,
                                this.contractType),
                            nameof(implementationType)),
                Func<IServiceProvider, object> factory =>
                    null,
                var instance =>
                    this.MatchesContractType(instance)
                        ? instance.GetType()
                        : throw new InvalidOperationException(
                            string.Format(
                                AbstractionStrings.AmbientServices_ServiceTypeAndServiceInstanceMismatch_Exception,
                                instance.GetType(),
                                this.contractType)),
            };

            this.instancingStrategy = instancingStrategy;
            if (this.contractType is null)
            {
                this.contractType = inferredContractType;
            }

            return this;
        }

        private bool MatchesContractType(object instance)
        {
            return this.contractType?.IsInstanceOfType(instance) ?? true;
        }

        private bool MatchesContractType(Type implementationType)
        {
            if (this.contractType is null)
            {
                return true;
            }

            return this.contractType.IsAssignableFrom(implementationType)
                   || (this.contractType.IsGenericTypeDefinition
                       && !implementationType.IsGenericTypeDefinition
                       && implementationType.GetInterfaces().Any(i =>
                           i.IsGenericType && ReferenceEquals(i.GetGenericTypeDefinition(), this.contractType)))
                   || (this.contractType.IsGenericTypeDefinition
                       && implementationType.IsGenericTypeDefinition
                       && implementationType.GetInterfaces().Any(i => i.Name == this.contractType.Name));
        }

        private void EnsureContractTypeMatchesImplementationType(Type contractType, Type implementationType)
        {
            switch (contractType.IsGenericTypeDefinition)
            {
                case true when !implementationType.IsGenericTypeDefinition:
                    throw new ArgumentException(
                        string.Format(LiteStrings.LiteRegistrationBuilder_EnsureContractTypeMatchesImplementationType_ImplementationTypeMustBeGenericTypeDefinition, implementationType, contractType),
                        nameof(implementationType));
                case false when implementationType.IsGenericTypeDefinition:
                    throw new ArgumentException(
                        string.Format(LiteStrings.LiteRegistrationBuilder_EnsureContractTypeMatchesImplementationType_ImplementationTypeMustNotBeGenericTypeDefinition, implementationType, contractType),
                        nameof(implementationType));
            }
        }
    }
}