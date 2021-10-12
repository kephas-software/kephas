// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceDescriptorBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service descriptor builder class.
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
    using Kephas.Services;

    /// <summary>
    /// A service descriptor builder.
    /// </summary>
    internal class ServiceDescriptorBuilder
    {
        private readonly ContainerBuilder containerBuilder;
        private IDictionary<string, object?>? metadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDescriptorBuilder"/> class.
        /// </summary>
        /// <param name="containerBuilder">The container builder.</param>
        public ServiceDescriptorBuilder(ContainerBuilder containerBuilder)
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
        /// Adds metadata.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddMetadata(string name, object? value)
        {
            (this.metadata ??= new Dictionary<string, object?>())[name] = value;
        }

        /// <summary>
        /// Adds metadata.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public void AddMetadata(IDictionary<string, object?> metadata)
        {
            (this.metadata ??= new Dictionary<string, object?>()).Merge(metadata);
        }

        /// <summary>
        /// Builds the information into a service descriptor.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
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