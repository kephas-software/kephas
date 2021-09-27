// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPartConventionsBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for part convention builders.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Conventions
{
    using System;

    /// <summary>
    /// Contract for part conventions builders.
    /// </summary>
    public interface IPartConventionsBuilder : IPartBuilder
    {
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
    }
}