// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemCompositionExportConventionsBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Export conventions builder for MEF.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.SystemComposition.Conventions
{
    using System;
    using System.Composition.Convention;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection.Conventions;

    /// <summary>
    /// Export conventions builder for MEF.
    /// </summary>
    public class SystemCompositionExportConventionsBuilder : IExportConventionsBuilder
    {
        /// <summary>
        /// The inner export convention builder.
        /// </summary>
        private readonly ExportConventionBuilder innerExportConventionBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemCompositionExportConventionsBuilder"/> class.
        /// </summary>
        /// <param name="innerExportConventionBuilder">The inner export convention builder.</param>
        internal SystemCompositionExportConventionsBuilder(ExportConventionBuilder innerExportConventionBuilder)
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
            Requires.NotNull(contractType, nameof(contractType));

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
            Requires.NotNullOrEmpty(name, nameof(name));

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
            Requires.NotNullOrEmpty(name, nameof(name));
            Requires.NotNull(getValueFromPartType, nameof(getValueFromPartType));

            this.innerExportConventionBuilder.AddMetadata(name, getValueFromPartType);

            return this;
        }
    }
}