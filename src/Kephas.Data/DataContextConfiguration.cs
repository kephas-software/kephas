// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextConfiguration.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data context configuration class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using Kephas.Services;

    /// <summary>
    /// A data context configuration.
    /// </summary>
    public class DataContextConfiguration : ContextBase, IDataContextConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextConfiguration"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public DataContextConfiguration(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public string ConnectionString { get; }
    }
}