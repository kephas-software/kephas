// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExportConventionsBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for export conventions builder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Kephas.Injection.Conventions
{
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

        /// <summary>
        /// Add export metadata to the export.
        /// </summary>
        /// <param name="name">The name of the metadata item.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>
        /// An export builder allowing further configuration.
        /// </returns>
        IExportConventionsBuilder AddMetadata(string name, object value);

        /// <summary>
        /// Add export metadata to the export.
        /// </summary>
        /// <param name="name">The name of the metadata item.</param>
        /// <param name="getValueFromPartType">A function that calculates the metadata value based on the type.</param>
        /// <returns>
        /// An export builder allowing further configuration.
        /// </returns>
        IExportConventionsBuilder AddMetadata(string name, Func<Type, object> getValueFromPartType);
    }
}