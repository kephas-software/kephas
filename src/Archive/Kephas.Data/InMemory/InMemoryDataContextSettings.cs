// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryDataContextSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the in memory data context settings class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.InMemory
{
    using System.Collections.Generic;
    using Kephas.Data;
    using Kephas.Data.Capabilities;

    /// <summary>
    /// An in memory data context settings.
    /// </summary>
    public class InMemoryDataContextSettings : DataContextSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryDataContextSettings"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public InMemoryDataContextSettings(string connectionString)
            : base(connectionString)
        {
        }

        /// <summary>
        /// Gets or sets the initial data.
        /// </summary>
        /// <value>
        /// The initial data.
        /// </value>
        public IEnumerable<IEntityEntry> InitialData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use the shared cache or not.
        /// </summary>
        /// <value>
        /// True if shared cache is used, false if not.
        /// </value>
        public bool? UseSharedCache { get; set; }
    }
}