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
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Contract for export conventions builder.
    /// </summary>
    [ContractClass(typeof(ExportConventionsBuilderContractClass))]
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

    /// <summary>
    /// Contract class for <see cref="IExportConventionsBuilder"/>.
    /// </summary>
    [ContractClassFor(typeof(IExportConventionsBuilder))]
    internal abstract class ExportConventionsBuilderContractClass : IExportConventionsBuilder
    {
        /// <summary>
        /// Specify the contract type for the export.
        /// </summary>
        /// <param name="contractType">The contract type.</param>
        /// <returns>
        /// An export builder allowing further configuration.
        /// </returns>
        public IExportConventionsBuilder AsContractType(Type contractType)
        {
            Contract.Requires(contractType != null);
            Contract.Ensures(Contract.Result<IExportConventionsBuilder>() != null);
            return Contract.Result<IExportConventionsBuilder>();
        }

        /// <summary>
        /// Add export metadata to the export.
        /// </summary>
        /// <param name="name">The name of the metadata item.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>
        /// An export builder allowing further configuration.
        /// </returns>
        public IExportConventionsBuilder AddMetadata(string name, object value)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));
            Contract.Ensures(Contract.Result<IExportConventionsBuilder>() != null);
            return Contract.Result<IExportConventionsBuilder>();
        }

        /// <summary>
        /// Add export metadata to the export.
        /// </summary>
        /// <param name="name">The name of the metadata item.</param>
        /// <param name="getValueFromPartType">A function that calculates the metadata value based on the type.</param>
        /// <returns>
        /// An export builder allowing further configuration.
        /// </returns>
        public IExportConventionsBuilder AddMetadata(string name, Func<Type, object> getValueFromPartType)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));
            Contract.Requires(getValueFromPartType != null);
            Contract.Ensures(Contract.Result<IExportConventionsBuilder>() != null);
            return Contract.Result<IExportConventionsBuilder>();
        }
    }
}