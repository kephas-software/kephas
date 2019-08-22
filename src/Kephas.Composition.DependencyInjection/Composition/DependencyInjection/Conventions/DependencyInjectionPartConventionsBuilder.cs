// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionPartConventionsBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the medi part conventions builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.DependencyInjection.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Composition.Conventions;
    using Kephas.Logging;

    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A Microsoft.Extensions.DependencyInjection part conventions builder.
    /// </summary>
    public class DependencyInjectionPartConventionsBuilder : IPartConventionsBuilder
    {
        private readonly ServiceDescriptorBuilder descriptorBuilder;

        private ILogger logger = typeof(DependencyInjectionPartConventionsBuilder).GetLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionPartConventionsBuilder"/> class.
        /// </summary>
        /// <param name="descriptorBuilder">The descriptor builder.</param>
        internal DependencyInjectionPartConventionsBuilder(ServiceDescriptorBuilder descriptorBuilder)
        {
            this.descriptorBuilder = descriptorBuilder;
        }

        /// <summary>
        /// Mark the part as being shared within the entire composition.
        /// </summary>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IPartConventionsBuilder Singleton()
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
        public IPartConventionsBuilder Scoped()
        {
            this.descriptorBuilder.Lifetime = ServiceLifetime.Scoped;
            return this;
        }

        /// <summary>
        /// Exports the part using the specified conventions builder.
        /// </summary>
        /// <param name="conventionsBuilder">Optional. The conventions builder.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IPartConventionsBuilder Export(Action<IExportConventionsBuilder> conventionsBuilder = null)
        {
            if (conventionsBuilder != null)
            {
                this.descriptorBuilder.ExportConfiguration = (t, b) => conventionsBuilder(b);
            }

            return this;
        }

        /// <summary>
        /// Select the interface on the part type that will be exported.
        /// </summary>
        /// <param name="exportInterface">The interface to export.</param>
        /// <param name="exportConfiguration">Optional. The export configuration.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IPartConventionsBuilder ExportInterface(Type exportInterface, Action<Type, IExportConventionsBuilder> exportConfiguration = null)
        {
            this.descriptorBuilder.ServiceType = exportInterface;
            this.descriptorBuilder.ExportConfiguration = exportConfiguration;

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
        public IPartConventionsBuilder SelectConstructor(Func<IEnumerable<ConstructorInfo>, ConstructorInfo> constructorSelector, Action<ParameterInfo, IImportConventionsBuilder> importConfiguration = null)
        {
            // TODO not supported.
            this.logger.Warn($"Selecting a specific constructor is not supported ({this.descriptorBuilder}).");
            return this;
        }

        /// <summary>
        /// Select properties to import into the part.
        /// </summary>
        /// <param name="propertyFilter">Filter to select matching properties.</param>
        /// <param name="importConfiguration">Optional. Action to configure selected properties.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IPartConventionsBuilder ImportProperties(Predicate<PropertyInfo> propertyFilter, Action<PropertyInfo, IImportConventionsBuilder> importConfiguration = null)
        {
            // TODO not supported.
            this.logger.Warn($"Property injection is not supported ({this.descriptorBuilder}).");
            throw new NotImplementedException();
        }
    }
}