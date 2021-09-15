// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPartConventionsBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for part convention builders.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kephas.Injection.Conventions
{
    /// <summary>
    /// Contract for part conventions builders.
    /// </summary>
    public interface IPartConventionsBuilder
    {
        /// <summary>
        /// Indicates the declared service type. Typically this is the same as the contract type, but
        /// this may get overwritten, for example when declaring generic type services for collecting
        /// metadata.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        IPartConventionsBuilder AsServiceType(Type serviceType);

        /// <summary>
        /// Mark the part as being shared within the entire composition.
        /// </summary>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        IPartConventionsBuilder Singleton();

        /// <summary>
        /// Mark the part as being shared within the scope.
        /// </summary>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        IPartConventionsBuilder Scoped();

        /// <summary>
        /// Exports the part using the specified conventions builder.
        /// </summary>
        /// <param name="conventionsBuilder">The conventions builder.</param>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        IPartConventionsBuilder Export(Action<IExportConventionsBuilder>? conventionsBuilder = null);

        /// <summary>
        /// Select the interface on the part type that will be exported.
        /// </summary>
        /// <param name="exportInterface">The interface to export.</param>
        /// <param name="exportConfiguration">The export configuration.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        IPartConventionsBuilder ExportInterface(
            Type exportInterface,
            Action<Type, IExportConventionsBuilder>? exportConfiguration = null);

        /// <summary>
        /// Select which of the available constructors will be used to instantiate the part.
        /// </summary>
        /// <param name="constructorSelector">Filter that selects a single constructor.</param><param name="importConfiguration">Action configuring the parameters of the selected constructor.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        IPartConventionsBuilder SelectConstructor(Func<IEnumerable<ConstructorInfo>, ConstructorInfo?> constructorSelector, Action<ParameterInfo, IImportConventionsBuilder>? importConfiguration = null);

        /// <summary>
        /// Indicates that this service allows multiple registrations.
        /// </summary>
        /// <param name="value">True if multiple service registrations are allowed, false otherwise.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        IPartConventionsBuilder AllowMultiple(bool value);
    }
}