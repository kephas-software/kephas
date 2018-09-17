// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataImportContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data import context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Import
{
    using System;

    using Kephas.Data;
    using Kephas.Data.Commands;
    using Kephas.Data.Conversion;
    using Kephas.Data.IO;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// A data import context.
    /// </summary>
    public class DataImportContext : DataIOContext, IDataImportContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataImportContext"/> class.
        /// </summary>
        /// <param name="dataSpace">The data space.</param>
        /// <param name="operationContext">Optional. Context for the operation.</param>
        public DataImportContext(IDataSpace dataSpace, IContext operationContext = null)
            : base(operationContext ?? dataSpace)
        {
            Requires.NotNull(dataSpace, nameof(dataSpace));

            this.DataSpace = dataSpace;
        }

        /// <summary>
        /// Gets the data space.
        /// </summary>
        /// <value>
        /// The data space.
        /// </value>
        public IDataSpace DataSpace { get; }

        /// <summary>
        /// Gets or sets the data conversion context configuration.
        /// </summary>
        /// <value>
        /// The data conversion context configuration.
        /// </value>
        public Action<object, IDataConversionContext> DataConversionContextConfig { get; set; }

        /// <summary>
        /// Gets or sets the persist changes context configuration.
        /// </summary>
        /// <value>
        /// The persist changes context configuration.
        /// </value>
        public Action<IPersistChangesContext> PersistChangesContextConfig { get; set; }
    }
}