// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFindContextBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IFindContextBase interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;

    /// <summary>
    /// Base contract for find commands.
    /// </summary>
    public interface IFindContextBase : IDataOperationContext
    {
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        Type EntityType { get; }

        /// <summary>
        /// Gets a value indicating whether to throw an exception if an entity is not found.
        /// </summary>
        /// <value>
        /// <c>true</c>true to throw an exception if an entity is not found, otherwise <c>false</c>.
        /// </value>
        bool ThrowIfNotFound { get; }
    }
}