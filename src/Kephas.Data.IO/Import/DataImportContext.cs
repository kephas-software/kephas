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

    using Kephas.Composition;
    using Kephas.Data;
    using Kephas.Data.Commands;
    using Kephas.Data.Conversion;
    using Kephas.Data.IO;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A data import context.
    /// </summary>
    public class DataImportContext : DataIOContext, IDataImportContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataImportContext"/> class.
        /// </summary>
        /// <param name="injector">The composition context.</param>
        public DataImportContext(IInjector injector)
            : base(injector)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataImportContext"/> class.
        /// </summary>
        /// <param name="dataSpace">The data space.</param>
        public DataImportContext(IDataSpace dataSpace)
            : base(dataSpace)
        {
            Requires.NotNull(dataSpace, nameof(dataSpace));

            this.DataSpace = dataSpace;
        }

        /// <summary>
        /// Gets or sets the data space.
        /// </summary>
        /// <value>
        /// The data space.
        /// </value>
        public IDataSpace DataSpace { get; set; }

        /// <summary>
        /// Gets or sets the data conversion options configuration.
        /// </summary>
        /// <value>
        /// The data conversion options configuration.
        /// </value>
        public Action<object, IDataConversionContext> DataConversionConfig { get; set; }

        /// <summary>
        /// Gets or sets the persist changes options configuration.
        /// </summary>
        /// <value>
        /// The persist changes options configuration.
        /// </value>
        public Action<IPersistChangesContext> PersistChangesConfig { get; set; }
    }
}