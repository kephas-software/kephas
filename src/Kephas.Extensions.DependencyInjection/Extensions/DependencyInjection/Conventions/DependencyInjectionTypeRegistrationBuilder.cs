// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionTypeRegistrationBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection.Builder;
    using Kephas.Logging;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A Microsoft.Extensions.DependencyInjection part conventions builder.
    /// </summary>
    public class DependencyInjectionTypeRegistrationBuilder : Loggable, IRegistrationBuilder
    {
        private readonly ServiceDescriptorBuilder descriptorBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionTypeRegistrationBuilder"/> class.
        /// </summary>
        /// <param name="descriptorBuilder">The descriptor builder.</param>
        internal DependencyInjectionTypeRegistrationBuilder(ServiceDescriptorBuilder descriptorBuilder)
        {
            this.descriptorBuilder = descriptorBuilder;
        }

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

            this.descriptorBuilder.ContractType = contractType;
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
            this.descriptorBuilder.Lifetime = ServiceLifetime.Singleton;
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
            this.descriptorBuilder.Lifetime = ServiceLifetime.Scoped;
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
            // TODO not supported.
            this.Logger.Warn($"Selecting a specific constructor is not supported ({this.descriptorBuilder}).");
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
            this.descriptorBuilder.AddMetadata(name, value);

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
            // not used, by default all services allow multiple instances.
            return this;
        }
    }
}