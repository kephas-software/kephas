// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Provides an indexer for getting and setting custom values.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    /// <summary>
    /// Provides an indexer for getting and setting custom values.
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="System.Object"/> with the specified key.</returns>
        object this[string key] { get; set; }
    }
}