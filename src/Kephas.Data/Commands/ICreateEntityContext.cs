// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICreateEntityContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ICreateEntityContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;

    /// <summary>
    /// Interface for create entity context.
    /// </summary>
    public interface ICreateEntityContext : IDataOperationContext
    {
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        Type EntityType { get; }
    }

    /// <summary>
    /// Generic interface for create entity context.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public interface ICreateEntityContext<T> : ICreateEntityContext
    {
    }
}