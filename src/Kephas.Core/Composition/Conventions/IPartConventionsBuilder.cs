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
    using System.Reflection;

    /// <summary>
    /// Contract for part conventions builders.
    /// </summary>
    public interface IPartConventionsBuilder
    {
        /// <summary>
        /// Mark the part as being shared within the entire composition.
        /// </summary>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        IPartConventionsBuilder Shared();

        /// <summary>
        /// Mark the part as being shared within the scope.
        /// </summary>
        /// <param name="scopeName">Name of the scope.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        IPartConventionsBuilder ScopeShared(string scopeName = ScopeNames.Default);

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
}