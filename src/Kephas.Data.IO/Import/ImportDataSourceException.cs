// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportDataSourceException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the import data source exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Import
{
    using System;

    using Kephas.Data.IO.DataStreams;
    using Kephas.Data.IO.Resources;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Exception for signalling import data source errors.
    /// </summary>
    public class ImportDataSourceException : DataIOException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportDataSourceException"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        public ImportDataSourceException(DataStream dataSource)
          : this(GetMessage(dataSource), dataSource)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportDataSourceException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="dataSource">The data source.</param>
        public ImportDataSourceException(string message, DataStream dataSource)
          : base(message)
        {
            Requires.NotNull(dataSource, nameof(dataSource));

            this.DataSource = dataSource;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportDataSourceException"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="inner">The inner.</param>
        public ImportDataSourceException(DataStream dataSource, Exception inner)
          : this(GetMessage(dataSource), dataSource, inner)
        {
            this.DataSource = dataSource;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportDataSourceException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="dataSource">The data source.</param>
        /// <param name="inner">The inner.</param>
        public ImportDataSourceException(string message, DataStream dataSource, Exception inner)
          : base(message, inner)
        {
            Requires.NotNull(dataSource, nameof(dataSource));

            this.DataSource = dataSource;
        }

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>
        /// The data source.
        /// </value>
        public DataStream DataSource { get; }

        /// <summary>
        /// Gets the formatted message.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <returns>The formatted message.</returns>
        private static string GetMessage(DataStream dataSource)
        {
            return string.Format(Strings.ImportDataSourceException_Message, dataSource);
        }
    }
}