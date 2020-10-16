// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataStore.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data store class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Store
{
    using System;

    using Kephas.Activation;
    using Kephas.Data;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Runtime;

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
        /// <param name="dataContextType">Optional. The type of the data context.</param>
        /// <param name="dataContextSettings">Optional. The data context settings.</param>
        /// <param name="entityActivator">Optional. The entity activator.</param>
        public DataStore(string name, string kind, Type? dataContextType = null, IDataContextSettings? dataContextSettings = null, IActivator? entityActivator = null)
        {
            Requires.NotNull(name, nameof(name));
            Requires.NotNullOrEmpty(kind, nameof(kind));

            this.Name = name;
            this.Kind = kind;
            this.DataContextType = dataContextType;
            this.DataContextSettings = dataContextSettings;
            this.EntityActivator = entityActivator ?? RuntimeActivator.Instance;
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
        public Type? DataContextType { get; }

        /// <summary>
        /// Gets the data context settings.
        /// </summary>
        /// <value>
        /// The data context settings.
        /// </value>
        public IDataContextSettings? DataContextSettings { get; }

        /// <summary>
        /// Gets the entity activator.
        /// </summary>
        /// <value>
        /// The entity activator.
        /// </value>
        public IActivator EntityActivator { get; }
    }
}