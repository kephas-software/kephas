﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionRegistrationBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the medi part builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Injection.Builder;
    using Kephas.Logging;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A Microsoft.Extensions.DependencyInjection part builder.
    /// </summary>
    public class DependencyInjectionRegistrationBuilder : Loggable, IRegistrationBuilder
    {
        private readonly ServiceDescriptorBuilder descriptorBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionRegistrationBuilder"/> class.
        /// </summary>
        /// <param name="descriptorBuilder">The descriptor builder.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        internal DependencyInjectionRegistrationBuilder(
            ServiceDescriptorBuilder descriptorBuilder,
            ILogManager? logManager = null)
            : base(logManager)
        {
            this.descriptorBuilder = descriptorBuilder;
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
        /// Indicates that this service allows multiple registrations.
        /// </summary>
        /// <param name="value">True if multiple service registrations are allowed, false otherwise.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        IRegistrationBuilder IRegistrationBuilder.AllowMultiple(bool value)
        {
            // By default, all registrations are multiple.
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
            // selecting a constructor is not supported.
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
        /// Indicates whether the created instances are disposed by an external owner.
        /// </summary>
        /// <returns>
        /// This builder.
        /// </returns>
        public IRegistrationBuilder ExternallyOwned()
        {
            this.Logger.Warn($"{nameof(DependencyInjectionRegistrationBuilder)} does not support externally owned instances.");
            return this;
        }
    }
}