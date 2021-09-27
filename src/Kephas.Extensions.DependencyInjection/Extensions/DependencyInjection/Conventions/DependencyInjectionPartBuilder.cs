// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionPartBuilder.cs" company="Kephas Software SRL">
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

    using Kephas.Injection.Conventions;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A Microsoft.Extensions.DependencyInjection part builder.
    /// </summary>
    public class DependencyInjectionPartBuilder : IPartBuilder
    {
        private readonly ServiceDescriptorBuilder descriptorBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionPartBuilder"/> class.
        /// </summary>
        /// <param name="descriptorBuilder">The descriptor builder.</param>
        internal DependencyInjectionPartBuilder(ServiceDescriptorBuilder descriptorBuilder)
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
        public IPartBuilder As(Type contractType)
        {
            this.descriptorBuilder.ServiceType = contractType;
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
            this.descriptorBuilder.Lifetime = ServiceLifetime.Singleton;
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
        IPartBuilder IPartBuilder.AllowMultiple(bool value)
        {
            // By default, all registrations are multiple.
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
            // TODO
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
    }
}