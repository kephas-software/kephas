// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataSourceContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataSourceContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.DataSources
{
    using Kephas.Reflection;

    /// <summary>
    /// Interface for data source context.
    /// </summary>
    public interface IDataSourceContext : IDataOperationContext
    {
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        ITypeInfo EntityType { get; }

        /// <summary>
        /// Gets the type of the projected entity.
        /// </summary>
        /// <value>
        /// The type of the projected entity.
        /// </value>
        ITypeInfo ProjectedEntityType { get; }

        /// <summary>
        /// Gets or sets options for controlling the operation.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        object Options { get; set; }
    }
}