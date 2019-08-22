// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceDescriptorBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service descriptor builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Autofac.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using global::Autofac;
    using global::Autofac.Builder;

    using Kephas.Composition.Conventions;
    using Kephas.Services;

    /// <summary>
    /// A service descriptor builder.
    /// </summary>
    internal class ServiceDescriptorBuilder
    {
        private readonly ContainerBuilder containerBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDescriptorBuilder"/> class.
        /// </summary>
        /// <param name="containerBuilder">The container builder.</param>
        public ServiceDescriptorBuilder(ContainerBuilder containerBuilder)
        {
            this.containerBuilder = containerBuilder;
        }

        /// <summary>
        /// Gets or sets the type of the service.
        /// </summary>
        /// <value>
        /// The type of the service.
        /// </value>
        public Type ServiceType { get; set; }

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
        public AppServiceLifetime Lifetime { get; set; } = AppServiceLifetime.Transient;

        /// <summary>
        /// Gets or sets the constructor selector.
        /// </summary>
        /// <value>
        /// A function delegate that yields a ConstructorInfo.
        /// </value>
        public Func<IEnumerable<ConstructorInfo>, ConstructorInfo> ConstructorSelector { get; set; }

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
        public void Build(IEnumerable<Type> parts)
        {
            if (this.ImplementationType != null)
            {
                this.RegisterService(this.ImplementationType);
                return;
            }

            if (this.ImplementationTypePredicate != null)
            {
                foreach (var type in parts.Where(t => this.ImplementationTypePredicate(t)))
                {
                    this.RegisterService(type);
                }

                return;
            }

            throw new InvalidOperationException(
                $"One of {nameof(ImplementationType)} or {nameof(ImplementationTypePredicate)} must be set.");
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
                                       ?? (this.ImplementationTypePredicate != null ? "type predicate" : "unknown");
            return $"{this.ServiceType}/{this.Lifetime}/{implementationString}";
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
                var registration = this.containerBuilder.RegisterType(implementationType);
                this.RegisterService(implementationType, registration);
            }
        }

        private void RegisterService<TActivatorData, TRegistrationStyle>(
            Type implementationType,
            IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> registration)
            where TActivatorData : ReflectionActivatorData
        {
            this.SetLifetime(registration);
            this.ExportConfiguration?.Invoke(implementationType, new ExportConventionsBuilder<TActivatorData, TRegistrationStyle>(this, implementationType, registration));
            if (this.ServiceType != null)
            {
                registration.As(this.ServiceType);
            }

            this.SelectConstructor(registration, implementationType);
        }

        private void SetLifetime<TActivatorData, TRegistrationStyle>(
            IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> registration)
        {
            switch (this.Lifetime)
            {
                case AppServiceLifetime.Singleton:
                    registration.SingleInstance();
                    break;
                case AppServiceLifetime.Scoped:
                    registration.InstancePerLifetimeScope();
                    break;
                default:
                    registration.InstancePerDependency();
                    break;
            }
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
            registration.UsingConstructor(constructor.GetParameters().Select(p => p.ParameterType).ToArray());
        }

        private class ExportConventionsBuilder<TActivatorData, TRegistrationStyle> : IExportConventionsBuilder
        {
            private readonly ServiceDescriptorBuilder descriptorBuilder;

            private readonly Type partType;

            private readonly IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> registration;

            public ExportConventionsBuilder(
                ServiceDescriptorBuilder descriptorBuilder,
                Type partType,
                IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> registration)
            {
                this.descriptorBuilder = descriptorBuilder;
                this.partType = partType;
                this.registration = registration;
            }

            public IExportConventionsBuilder AsContractType(Type contractType)
            {
                this.descriptorBuilder.ServiceType = contractType;

                return this;
            }

            public IExportConventionsBuilder AddMetadata(string name, object value)
            {
                this.registration.WithMetadata(name, value);
                return this;
            }

            public IExportConventionsBuilder AddMetadata(string name, Func<Type, object> getValueFromPartType)
            {
                this.registration.WithMetadata(name, getValueFromPartType(this.partType));
                return this;
            }
        }
    }
}