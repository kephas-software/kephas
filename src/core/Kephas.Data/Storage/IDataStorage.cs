// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataStorage.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Provides methods to persist and query entities in a storage.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Storage
{
    using System;

    using Kephas.Model;

    /// <summary>
    /// Provides methods to persist and query entities in a storage.
    /// </summary>
    public interface IDataStorage : IDisposable
    {
        /// <summary>
        /// Gets the model for this repository.
        /// </summary>
        /// <remarks>Each model corresponds to at most one data storage.</remarks>
        /// <value>
        /// The model.
        /// </value>
        IModelDimension Model { get; }
    }
}