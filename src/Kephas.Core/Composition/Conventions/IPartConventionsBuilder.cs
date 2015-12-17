// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPartConventionsBuilder.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for part convention builders.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    /// <summary>
    /// Contract for part conventions builders.
    /// </summary>
    [ContractClass(typeof(PartConventionsBuilderContractClass))]
    public interface IPartConventionsBuilder
    {
        /// <summary>
        /// Mark the part as being shared within the entire composition.
        /// </summary>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        IPartConventionsBuilder Shared();

        /// <summary>
        /// Exports the part using the specified conventions builder.
        /// </summary>
        /// <param name="conventionsBuilder">The conventions builder.</param>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        IPartConventionsBuilder Export(Action<IExportConventionsBuilder> conventionsBuilder = null);

        /// <summary>
        /// Select the interfaces on the part type that will be exported.
        /// </summary>
        /// <param name="interfaceFilter">The interface filter.</param>
        /// <param name="exportConfiguration">The export configuration.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        IPartConventionsBuilder ExportInterfaces(Predicate<Type> interfaceFilter = null, Action<Type, IExportConventionsBuilder> exportConfiguration = null);

        /// <summary>
        /// Select which of the available constructors will be used to instantiate the part.
        /// </summary>
        /// <param name="constructorSelector">Filter that selects a single constructor.</param><param name="importConfiguration">Action configuring the parameters of the selected constructor.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        IPartConventionsBuilder SelectConstructor(Func<IEnumerable<ConstructorInfo>, ConstructorInfo> constructorSelector, Action<ParameterInfo, IImportConventionsBuilder> importConfiguration = null);

        /// <summary>
        /// Select properties to import into the part.
        /// </summary>
        /// <param name="propertyFilter">Filter to select matching properties.</param>
        /// <param name="importConfiguration">Action to configure selected properties.</param>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        IPartConventionsBuilder ImportProperties(Predicate<PropertyInfo> propertyFilter, Action<PropertyInfo, IImportConventionsBuilder> importConfiguration = null);
    }

    /// <summary>
    /// Contract class for <see cref="IPartConventionsBuilder"/>.
    /// </summary>
    [ContractClassFor(typeof(IPartConventionsBuilder))]
    internal abstract class PartConventionsBuilderContractClass : IPartConventionsBuilder
    {
        /// <summary>
        /// Mark the part as being shared within the entire composition.
        /// </summary>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        public IPartConventionsBuilder Shared()
        {
            Contract.Ensures(Contract.Result<IPartConventionsBuilder>() != null);
            throw new NotSupportedException();
        }

        /// <summary>
        /// Exports the part using the specified conventions builder.
        /// </summary>
        /// <param name="conventionsBuilder">The conventions builder.</param>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        public IPartConventionsBuilder Export(Action<IExportConventionsBuilder> conventionsBuilder = null)
        {
            Contract.Ensures(Contract.Result<IPartConventionsBuilder>() != null);
            throw new NotSupportedException();
        }

        /// <summary>
        /// Select the interfaces on the part type that will be exported.
        /// </summary>
        /// <param name="interfaceFilter">The interface filter.</param>
        /// <param name="exportConfiguration">The export configuration.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IPartConventionsBuilder ExportInterfaces(Predicate<Type> interfaceFilter, Action<Type, IExportConventionsBuilder> exportConfiguration = null)
        {
            Contract.Ensures(Contract.Result<IPartConventionsBuilder>() != null);
            throw new NotSupportedException();
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
            Contract.Requires(constructorSelector != null);
            Contract.Ensures(Contract.Result<IPartConventionsBuilder>() != null);
            throw new NotSupportedException();
        }

        /// <summary>
        /// Select properties to import into the part.
        /// </summary>
        /// <param name="propertyFilter">Filter to select matching properties.</param>
        /// <param name="importConfiguration">Action to configure selected properties.</param>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        public IPartConventionsBuilder ImportProperties(Predicate<PropertyInfo> propertyFilter, Action<PropertyInfo, IImportConventionsBuilder> importConfiguration = null)
        {
            Contract.Requires(propertyFilter != null);
            Contract.Ensures(Contract.Result<IPartConventionsBuilder>() != null);
            throw new NotSupportedException();
        }
    }
}