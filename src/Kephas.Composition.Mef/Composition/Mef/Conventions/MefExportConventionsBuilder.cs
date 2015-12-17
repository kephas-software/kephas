// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefExportConventionsBuilder.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Export conventions builder for MEF.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.Conventions
{
    using System;
    using System.Composition.Convention;

    using Kephas.Composition.Conventions;

    /// <summary>
    /// Export conventions builder for MEF.
    /// </summary>
    public class MefExportConventionsBuilder : IExportConventionsBuilder
    {
        /// <summary>
        /// The inner export convention builder.
        /// </summary>
        private readonly ExportConventionBuilder innerExportConventionBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="MefExportConventionsBuilder"/> class.
        /// </summary>
        /// <param name="innerExportConventionBuilder">The inner export convention builder.</param>
        internal MefExportConventionsBuilder(ExportConventionBuilder innerExportConventionBuilder)
        {
            this.innerExportConventionBuilder = innerExportConventionBuilder;
        }

        /// <summary>
        /// Specify the contract type for the export.
        /// </summary>
        /// <param name="contractType">The contract type.</param>
        /// <returns>
        /// An export builder allowing further configuration.
        /// </returns>
        public IExportConventionsBuilder AsContractType(Type contractType)
        {
            this.innerExportConventionBuilder.AsContractType(contractType);

            return this;
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
            this.innerExportConventionBuilder.AddMetadata(name, value);

            return this;
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
            this.innerExportConventionBuilder.AddMetadata(name, getValueFromPartType);

            return this;
        }
    }
}