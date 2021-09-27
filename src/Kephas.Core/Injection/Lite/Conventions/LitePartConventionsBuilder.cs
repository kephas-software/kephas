// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LitePartConventionsBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the lite part conventions builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Lite.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection.Conventions;
    using Kephas.Logging;
    using Kephas.Services;

    /// <summary>
    /// A lightweight part conventions builder.
    /// </summary>
    internal class LitePartConventionsBuilder : Loggable, IPartBuilder
    {
        private readonly LiteRegistrationBuilder descriptorBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="LitePartConventionsBuilder"/> class.
        /// </summary>
        /// <param name="logManager">Manager for log.</param>
        /// <param name="descriptorBuilder">The descriptor builder.</param>
        internal LitePartConventionsBuilder(ILogManager logManager, LiteRegistrationBuilder descriptorBuilder)
            : base(logManager)
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
        public IPartBuilder As(Type contractType)
        {
            Requires.NotNull(contractType, nameof(contractType));

            this.descriptorBuilder.ContractType = contractType;
            return this;
        }

        /// <summary>
        /// Mark the part as being shared within the entire composition.
        /// </summary>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IPartBuilder Singleton()
        {
            this.descriptorBuilder.Lifetime = AppServiceLifetime.Singleton;
            return this;
        }

        /// <summary>
        /// Mark the part as being shared within the scope.
        /// </summary>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IPartBuilder Scoped()
        {
            this.descriptorBuilder.Lifetime = AppServiceLifetime.Scoped;
            return this;
        }

        /// <summary>
        /// Select which of the available constructors will be used to instantiate the part.
        /// </summary>
        /// <param name="constructorSelector">Filter that selects a single constructor.</param>
        /// <param name="importConfiguration">Optional. Action configuring the parameters of the selected
        ///                                   constructor.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IPartBuilder SelectConstructor(Func<IEnumerable<ConstructorInfo>, ConstructorInfo?> constructorSelector, Action<ParameterInfo, IImportConventionsBuilder>? importConfiguration = null)
        {
            // TODO not supported.
            if (this.Logger.IsTraceEnabled())
            {
                this.Logger.Warn("Selecting a specific constructor is not supported ({registrationBuilder}).", this.descriptorBuilder);
            }

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
        public IPartBuilder AllowMultiple(bool value)
        {
            this.descriptorBuilder.AllowMultiple = value;

            return this;
        }
    }
}