// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryDataContextConfiguration.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the in memory data context configuration class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.InMemory
{
    using System.Collections.Generic;

    using Kephas.Data.Capabilities;

    /// <summary>
    /// An in memory data context configuration.
    /// </summary>
    public class InMemoryDataContextConfiguration : DataContextConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryDataContextConfiguration"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public InMemoryDataContextConfiguration(string connectionString)
            : base(connectionString)
        {
        }

        /// <summary>
        /// Gets or sets the initial data.
        /// </summary>
        /// <value>
        /// The initial data.
        /// </value>
        public IEnumerable<IEntityInfo> InitialData { get; set; }
    }
}