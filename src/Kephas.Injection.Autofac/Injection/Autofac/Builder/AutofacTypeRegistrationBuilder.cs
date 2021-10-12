// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacTypeRegistrationBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the autofac part conventions builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Autofac.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using global::Autofac;
    using global::Autofac.Builder;
    using Kephas.Collections;
    using Kephas.Injection.Builder;
    using Kephas.Logging;
    using Kephas.Services;

    /// <summary>
    /// An Autofac part conventions builder.
    /// </summary>
    public class AutofacTypeRegistrationBuilder : Loggable, IAutofacRegistrationBuilder
    {
        private readonly ContainerBuilder containerBuilder;
        private IDictionary<string, object?>? metadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacTypeRegistrationBuilder"/> class.
        /// </summary>
        /// <param name="containerBuilder">The container builder.</param>
        /// <param name="logManager">The log manager.</param>
        internal AutofacTypeRegistrationBuilder(
            ContainerBuilder containerBuilder,
            ILogManager? logManager = null)
            : base(logManager)
        {
            this.containerBuilder = containerBuilder;
        }

        /// <summary>
        /// Gets or sets the contract type (service key).
        /// </summary>
        /// <value>
        /// The contract type (service key).
        /// </value>
        public Type? ContractType { get; set; }

        /// <summary>
        /// Gets or sets the type of the implementation.
        /// </summary>
        /// <value>
        /// The type of the implementation.
        /// </value>
        public Type? ImplementationType { get; set; }

        /// <summary>
        /// Gets or sets the lifetime.
        /// </summary>
        /// <value>
        /// The lifetime.
        /// </value>
        public AppServiceLifetime Lifetime { get; set; } = AppServiceLifetime.Transient;

        /// <summary>
        /// Gets or sets the constructor selector.
        /// </summary>
        /// <value>
        /// A function delegate that yields a ConstructorInfo.
        /// </value>
        public Func<IEnumerable<ConstructorInfo>, ConstructorInfo?>? ConstructorSelector { get; set; }

        /// <summary>
        /// Indicates the declared service type. Typically this is the same as the contract type, but
        /// this may get overwritten, for example when declaring generic type services for collecting
        /// metadata.
        /// </summary>
        /// <param name="contractType">Type of the service.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IRegistrationBuilder As(Type contractType)
        {
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));

            this.ContractType = contractType;
            return this;
        }

        /// <summary>
        /// Mark the part as being shared within the entire composition.
        /// </summary>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IRegistrationBuilder Singleton()
        {
            this.Lifetime = AppServiceLifetime.Singleton;
            return this;
        }

        /// <summary>
        /// Mark the part as being shared within the scope.
        /// </summary>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IRegistrationBuilder Scoped()
        {
            this.Lifetime = AppServiceLifetime.Scoped;
            return this;
        }

        /// <summary>
        /// Select which of the available constructors will be used to instantiate the part.
        /// </summary>
        /// <param name="constructorSelector">Filter that selects a single constructor.</param>
        /// <param name="parameterBuilder">The parameter builder.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IRegistrationBuilder SelectConstructor(
            Func<IEnumerable<ConstructorInfo>, ConstructorInfo?> constructorSelector,
            Action<ParameterInfo, IParameterBuilder>? parameterBuilder = null)
        {
            this.ConstructorSelector = constructorSelector;

            return this;
        }

        /// <summary>
        /// Adds metadata to the export.
        /// </summary>
        /// <param name="name">The name of the metadata item.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>
        /// A part builder allowing further configuration.
        /// </returns>
        public IRegistrationBuilder AddMetadata(string name, object? value)
        {
            (this.metadata ??= new Dictionary<string, object?>())[name] = value;
            return this;
        }

        /// <summary>
        /// Adds metadata to the export.
        /// </summary>
        /// <param name="metadata">The metadata dictionary.</param>
        /// <returns>
        /// A part builder allowing further configuration.
        /// </returns>
        public IRegistrationBuilder AddMetadata(IDictionary<string, object?> metadata)
        {
            (this.metadata ??= new Dictionary<string, object?>()).Merge(metadata);
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
            this.Logger.Warn($"{nameof(AutofacTypeRegistrationBuilder)} does not support externally owned instances, as it creates them.");
            return this;
        }

        /// <summary>
        /// Indicates that this service allows multiple registrations.
        /// </summary>
        /// <param name="value">True if multiple service registrations are allowed, false otherwise.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        IRegistrationBuilder IRegistrationBuilder.AllowMultiple(bool value)
        {
            // By default Autofac allows multiple registrations
            return this;
        }

        /// <summary>
        /// Builds the information into a service descriptor.
        /// </summary>
        public void Build()
        {
            if (this.ImplementationType != null)
            {
                this.RegisterService(this.ImplementationType);
                return;
            }

            throw new InvalidOperationException(
                $"{nameof(this.ImplementationType)} must be set to build this descriptor.");
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{this.ContractType}/{this.Lifetime}/{this.ImplementationType}";
        }

        private void RegisterService(Type implementationType)
        {
            if (implementationType.IsGenericTypeDefinition)
            {
                var registration = this.containerBuilder.RegisterGeneric(implementationType);
                this.RegisterService(implementationType, registration);
            }
            else
            {
                var registration = this.containerBuilder.RegisterType(implementationType).PreserveExistingDefaults();
                this.RegisterService(implementationType, registration);
            }
        }

        private IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> RegisterService<TActivatorData, TRegistrationStyle>(
            Type implementationType,
            IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> registration)
            where TActivatorData : ReflectionActivatorData
        {
            this.SetLifetime(registration);
            if (this.ContractType != null)
            {
                registration.As(this.ContractType);
            }

            this.SelectConstructor(registration, implementationType);

            if (this.metadata != null)
            {
                registration.WithMetadata(this.metadata);
            }

            return registration;
        }

        private IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> SetLifetime<TActivatorData, TRegistrationStyle>(
            IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> registration)
        {
            return this.Lifetime switch
            {
                AppServiceLifetime.Singleton => registration.SingleInstance(),
                AppServiceLifetime.Scoped => registration.InstancePerLifetimeScope(),
                _ => registration.InstancePerDependency()
            };
        }

        private void SelectConstructor<TActivatorData, TRegistrationStyle>(
            IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> registration,
            Type type)
            where TActivatorData : ReflectionActivatorData
        {
            if (this.ConstructorSelector == null)
            {
                return;
            }

            var constructor = this.ConstructorSelector(type.GetConstructors());
            if (constructor == null)
            {
                // no constructor selected, it means we leave the selection up to Autofac.
                return;
            }

            registration.UsingConstructor(constructor.GetParameters().Select(p => p.ParameterType).ToArray());
        }
    }
}