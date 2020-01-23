// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data context settings class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using Kephas.Dynamic;

    /// <summary>
    /// A data context settings.
    /// </summary>
    public class DataContextSettings : Expando, IDataContextSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextSettings"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public DataContextSettings(string connectionString)
        {
            ConnectionString = connectionString;
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