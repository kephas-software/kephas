// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextConfiguration.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data context configuration class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using Kephas.Composition;
    using Kephas.Services;

    /// <summary>
    /// A data context configuration.
    /// </summary>
    public class DataContextConfiguration : Context, IDataContextConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextConfiguration"/> class.
        /// </summary>
        /// <param name="compositionContext">Context for the composition.</param>
        /// <param name="connectionString">The connection string.</param>
        public DataContextConfiguration(
            ICompositionContext compositionContext,
            string connectionString)
            : base(compositionContext)
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