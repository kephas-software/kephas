// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefPartConventionsBuilder.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    using System.Diagnostics.Contracts;
    using System.Reflection;

    using Kephas.Composition.Conventions;

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
            Contract.Requires(innerConventionBuilder != null);

            this.innerConventionBuilder = innerConventionBuilder;
        }
        
        /// <summary>
        /// Mark the part as being shared within the entire composition.
        /// </summary>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        public IPartConventionsBuilder Shared()
        {
            this.innerConventionBuilder.Shared();
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
        /// Select the interfaces on the part type that will be exported.
        /// </summary>
        /// <param name="interfaceFilter">The interface filter.</param>
        /// <param name="exportConfiguration">The export configuration.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IPartConventionsBuilder ExportInterfaces(Predicate<Type> interfaceFilter = null, Action<Type, IExportConventionsBuilder> exportConfiguration = null)
        {
            if (interfaceFilter == null && exportConfiguration != null)
            {
                throw new ArgumentException("If an export configuration is specified, then you must also specify an interface filter.");
            }

            if (interfaceFilter == null)
            {
                this.innerConventionBuilder.ExportInterfaces();
            }
            else if (exportConfiguration == null)
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
    }
}