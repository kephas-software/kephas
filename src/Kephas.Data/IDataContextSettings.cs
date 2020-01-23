// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataContextSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataContextSettings interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using Kephas.Dynamic;

    /// <summary>
    /// Interface for data context settings.
    /// </summary>
    public interface IDataContextSettings : IExpando
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        string ConnectionString { get; }
    }
}