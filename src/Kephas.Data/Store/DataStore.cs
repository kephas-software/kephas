// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataStore.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data store class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Store
{
    using System;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A data store.
    /// </summary>
    public class DataStore : IDataStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataStore"/> class.
        /// </summary>
        /// <param name="name">The data store name.</param>
        /// <param name="kind">The data store kind.</param>
        /// <param name="dataContextType">The type of the data context (optional).</param>
        /// <param name="dataContextConfiguration">The data context configuration (optional).</param>
        public DataStore(string name, string kind, Type dataContextType = null, IDataContextConfiguration dataContextConfiguration = null)
        {
            Requires.NotNull(name, nameof(name));
            Requires.NotNullOrEmpty(kind, nameof(kind));

            this.Name = name;
            this.Kind = kind;
            this.DataContextType = dataContextType;
            this.DataContextConfiguration = dataContextConfiguration;
        }

        /// <summary>
        /// Gets the data store name.
        /// </summary>
        /// <value>
        /// The data store name.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the data store kind.
        /// </summary>
        /// <value>
        /// The data store kind.
        /// </value>
        public string Kind { get; }

        /// <summary>
        /// Gets the type of the data context.
        /// </summary>
        /// <value>
        /// The type of the data context.
        /// </value>
        public Type DataContextType { get; }

        /// <summary>
        /// Gets the data context configuration.
        /// </summary>
        /// <value>
        /// The data context configuration.
        /// </value>
        public IDataContextConfiguration DataContextConfiguration { get; }
    }
}