// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefPartConventionsBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Conventions builder for a specific part.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Convention;
    using System.Reflection;

    using Kephas.Composition.Conventions;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Conventions builder for a specific part.
    /// </summary>
    public class MefPartConventionsBuilder : IPartConventionsBuilder
    {
        /// <summary>
        /// The inner convention builder.
        /// </summary>
        private readonly PartConventionBuilder innerConventionBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="MefPartConventionsBuilder"/> class.
        /// </summary>
        /// <param name="innerConventionBuilder">The inner convention builder.</param>
        internal MefPartConventionsBuilder(PartConventionBuilder innerConventionBuilder)
        {
            Requires.NotNull(innerConventionBuilder, nameof(innerConventionBuilder));

            this.innerConventionBuilder = innerConventionBuilder;
        }

        /// <summary>
        /// Mark the part as being shared within the entire composition.
        /// </summary>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        public IPartConventionsBuilder Singleton()
        {
            this.innerConventionBuilder.Shared();
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
            this.innerConventionBuilder.Shared(CompositionScopeNames.Default);
            return this;
        }

        /// <summary>
        /// Exports the part using the specified conventions builder.
        /// </summary>
        /// <param name="conventionsBuilder">The conventions builder.</param>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        public IPartConventionsBuilder Export(Action<IExportConventionsBuilder> conventionsBuilder = null)
        {
            if (conventionsBuilder == null)
            {
                this.innerConventionBuilder.Export();
            }
            else
            {
                this.innerConventionBuilder.Export(b => conventionsBuilder(new MefExportConventionsBuilder(b)));
            }

            return this;
        }

        /// <summary>
        /// Select the interface on the part type that will be exported.
        /// </summary>
        /// <param name="exportInterface">The interface to export.</param>
        /// <param name="exportConfiguration">The export configuration.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IPartConventionsBuilder ExportInterface(
            Type exportInterface,
            Action<Type, IExportConventionsBuilder> exportConfiguration = null)
        {
            Requires.NotNull(exportInterface, nameof(exportInterface));

            var interfaceFilter = exportInterface.IsGenericTypeDefinition
                                      ? (Predicate<Type>)(t => this.IsClosedGenericOf(exportInterface, t))
                                      : t => ReferenceEquals(exportInterface, t);

            if (exportConfiguration == null)
            {
                this.innerConventionBuilder.ExportInterfaces(interfaceFilter);
            }
            else
            {
                this.innerConventionBuilder.ExportInterfaces(
                    interfaceFilter,
                    (t, builder) => exportConfiguration(t, new MefExportConventionsBuilder(builder)));
            }

            return this;
        }

        /// <summary>
        /// Select which of the available constructors will be used to instantiate the part.
        /// </summary>
        /// <param name="constructorSelector">Filter that selects a single constructor.</param><param name="importConfiguration">Action configuring the parameters of the selected constructor.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IPartConventionsBuilder SelectConstructor(Func<IEnumerable<ConstructorInfo>, ConstructorInfo> constructorSelector, Action<ParameterInfo, IImportConventionsBuilder> importConfiguration = null)
        {
            Requires.NotNull(constructorSelector, nameof(constructorSelector));

            if (importConfiguration == null)
            {
                this.innerConventionBuilder.SelectConstructor(constructorSelector);
            }
            else
            {
                this.innerConventionBuilder.SelectConstructor(
                    constructorSelector,
                    (pi, config) => importConfiguration(pi, new MefImportConventionsBuilder(config)));
            }

            return this;
        }

        /// <summary>
        /// Select properties to import into the part.
        /// </summary>
        /// <param name="propertyFilter">Filter to select matching properties.</param>
        /// <param name="importConfiguration">Action to configure selected properties.</param>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        public IPartConventionsBuilder ImportProperties(Predicate<PropertyInfo> propertyFilter, Action<PropertyInfo, IImportConventionsBuilder> importConfiguration = null)
        {
            Requires.NotNull(propertyFilter, nameof(propertyFilter));

            if (importConfiguration == null)
            {
                this.innerConventionBuilder.ImportProperties(propertyFilter);
            }
            else
            {
                this.innerConventionBuilder.ImportProperties(
                    propertyFilter,
                    (pi, config) => importConfiguration(pi, new MefImportConventionsBuilder(config)));
            }

            return this;
        }

        /// <summary>
        /// Determines whether the provided interface is a closed generic of the specified open generic contract.
        /// </summary>
        /// <param name="openGenericContract">The open generic contract.</param>
        /// <param name="exportInterface">The export interface.</param>
        /// <returns><c>true</c> if the provided interface is a closed generic of the specified open generic contract, otherwise <c>false</c>.</returns>
        private bool IsClosedGenericOf(Type openGenericContract, Type exportInterface)
        {
            return exportInterface.IsGenericType && exportInterface.GetGenericTypeDefinition() == openGenericContract;
        }
    }
}