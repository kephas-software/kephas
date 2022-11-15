// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacInstanceRegistrationBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Autofac.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using global::Autofac;
    using Kephas.Collections;
    using Kephas.Injection.Builder;

    /// <summary>
    /// An Autofac part builder.
    /// </summary>
    public class AutofacInstanceRegistrationBuilder : IAutofacRegistrationBuilder
    {
        private readonly ContainerBuilder containerBuilder;
        private readonly object instance;

        private readonly bool preserveRegistrationOrder;
        private Type? contractType;
        private IDictionary<string, object?>? metadata;
        private bool isExternallyOwned;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacInstanceRegistrationBuilder"/> class.
        /// </summary>
        /// <param name="containerBuilder">The container builder.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="preserveRegistrationOrder">Optional. Indicates whether to preserve the registration order. Relevant for integration with ASP.NET Core.</param>
        public AutofacInstanceRegistrationBuilder(
            ContainerBuilder containerBuilder,
            object instance,
            bool preserveRegistrationOrder)
        {
            this.containerBuilder = containerBuilder ?? throw new ArgumentNullException(nameof(containerBuilder));
            this.instance = instance ?? throw new ArgumentNullException(nameof(instance));
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
            this.contractType = contractType;
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
            // always singleton
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
            // always singleton
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
            // selecting a constructor is not supported for instance
            return this;
        }

        /// <summary>
        /// Adds metadata to the export.
        /// </summary>
        /// <param name="name">The name of the metadata item.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>
        /// A registration builder allowing further configuration.
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
            this.isExternallyOwned = true;
            return this;
        }

        /// <summary>
        /// Builds the information into a service descriptor.
        /// </summary>
        public void Build()
        {
            var registrationBuilder = this.containerBuilder.RegisterInstance(this.instance);
            if (this.contractType != null)
            {
                registrationBuilder.As(this.contractType);
            }

            if (this.metadata != null)
            {
                registrationBuilder.WithMetadata(this.metadata);
            }

            if (this.isExternallyOwned)
            {
                registrationBuilder.ExternallyOwned();
            }

            if (this.preserveRegistrationOrder)
            {
                registrationBuilder.PreserveExistingDefaults();
            }
        }
    }
}