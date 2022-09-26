﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacFactoryRegistrationBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the autofac part builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Autofac.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using global::Autofac;
    using global::Autofac.Builder;
    using Kephas.Injection.Builder;
    using Kephas.Services.Reflection;

    /// <summary>
    /// An Autofac part builder.
    /// </summary>
    public class AutofacFactoryRegistrationBuilder : IAutofacRegistrationBuilder
    {
        private readonly ContainerBuilder containerBuilder;

        private readonly IRegistrationBuilder<object, SimpleActivatorData, SingleRegistrationStyle> registrationBuilder;
        private readonly bool preserveRegistrationOrder;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacFactoryRegistrationBuilder"/> class.
        /// </summary>
        /// <param name="containerBuilder">The container builder.</param>
        /// <param name="type">The type of the instance returned by the factory function.</param>
        /// <param name="factory">The factory function.</param>
        /// <param name="preserveRegistrationOrder">Optional. Indicates whether to preserve the registration order. Relevant for integration with ASP.NET Core.</param>
        public AutofacFactoryRegistrationBuilder(
            ContainerBuilder containerBuilder,
            Type type,
            Func<IInjector, object> factory,
            bool preserveRegistrationOrder)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));
            factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.containerBuilder = containerBuilder ?? throw new ArgumentNullException(nameof(containerBuilder));
            this.registrationBuilder = RegistrationBuilder.ForDelegate(
                type,
                (context, _) => factory(context.Resolve<IInjector>()));
            this.registrationBuilder.As(type);
            this.preserveRegistrationOrder = preserveRegistrationOrder;
        }

        /// <summary>
        /// Indicates the type registered as the exported service key.
        /// </summary>
        /// <param name="contractType">Type of the service.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IRegistrationBuilder As(Type contractType)
        {
            this.registrationBuilder.As(contractType);
            return this;
        }

        /// <summary>
        /// Marks the part as singleton within the injection.
        /// </summary>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IRegistrationBuilder Singleton()
        {
            this.registrationBuilder.SingleInstance();
            return this;
        }

        /// <summary>
        /// Mark the part as singleton within a scope.
        /// </summary>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IRegistrationBuilder Scoped()
        {
            this.registrationBuilder.InstancePerLifetimeScope();
            return this;
        }

        /// <summary>
        /// Indicates that this service allows multiple registrations.
        /// </summary>
        /// <param name="value">True if multiple service registrations are allowed, false otherwise.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IRegistrationBuilder AllowMultiple(bool value)
        {
            // by default Autofac allows multiple services.
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
            // selecting a constructor is not supported for instance and factory
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
            this.registrationBuilder.WithMetadata(name, value);
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
            this.registrationBuilder.WithMetadata(metadata);
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
            this.registrationBuilder.ExternallyOwned();
            return this;
        }

        /// <summary>
        /// Builds the information into a service descriptor.
        /// </summary>
        public void Build()
        {
            if (this.preserveRegistrationOrder)
            {
                this.registrationBuilder.PreserveExistingDefaults();
            }

            var registration = this.registrationBuilder.CreateRegistration();
            this.containerBuilder.RegisterComponent(registration);
        }
    }
}