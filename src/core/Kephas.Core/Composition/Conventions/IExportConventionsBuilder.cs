// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExportConventionsBuilder.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for export conventions builder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Conventions
{
    using System;

    /// <summary>
    /// Contract for export conventions builder.
    /// </summary>
    public interface IExportConventionsBuilder
    {
        /// <summary>
        /// Specify the contract type for the export.
        /// </summary>
        /// <param name="contractType">The contract type.</param>
        /// <returns>
        /// An export builder allowing further configuration.
        /// </returns>
        IExportConventionsBuilder AsContractType(Type contractType);
    }
}