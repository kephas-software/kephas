// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacSimplePartBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the autofac part builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Autofac.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using global::Autofac;
    using global::Autofac.Builder;
    using Kephas.Injection.Conventions;

    /// <summary>
    /// An Autofac part builder.
    /// </summary>
    public class AutofacSimplePartBuilder : IPartBuilder
    {
        private readonly ContainerBuilder containerBuilder;

        private readonly IRegistrationBuilder<object, SimpleActivatorData, SingleRegistrationStyle> registrationBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacSimplePartBuilder"/> class.
        /// </summary>
        /// <param name="containerBuilder">The container builder.</param>
        /// <param name="registrationBuilder">The registration builder.</param>
        public AutofacSimplePartBuilder(ContainerBuilder containerBuilder, IRegistrationBuilder<object, SimpleActivatorData, SingleRegistrationStyle> registrationBuilder)
        {
            this.containerBuilder = containerBuilder;
            this.registrationBuilder = registrationBuilder;
        }

        /// <summary>
        /// Indicates the type registered as the exported service key.
        /// </summary>
        /// <param name="contractType">Type of the service.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IPartBuilder As(Type contractType)
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
        public IPartBuilder Singleton()
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
        public IPartBuilder Scoped()
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
        public IPartBuilder AllowMultiple(bool value)
        {
            // by default aAutofac allows multiple services.
            return this;
        }

        /// <summary>
        /// Select which of the available constructors will be used to instantiate the part.
        /// </summary>
        /// <param name="constructorSelector">Filter that selects a single constructor.</param><param name="importConfiguration">Action configuring the parameters of the selected constructor.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IPartBuilder SelectConstructor(Func<IEnumerable<ConstructorInfo>, ConstructorInfo?> constructorSelector, Action<ParameterInfo, IImportConventionsBuilder>? importConfiguration = null)
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
        public IPartBuilder AddMetadata(string name, object? value)
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
        public IPartBuilder AddMetadata(IDictionary<string, object?> metadata)
        {
            this.registrationBuilder.WithMetadata(metadata);
            return this;
        }

        /// <summary>
        /// Builds the information into a service descriptor.
        /// </summary>
        public void Build()
        {
            var registration = this.registrationBuilder.CreateRegistration();
            this.containerBuilder.RegisterComponent(registration);
        }
    }
}