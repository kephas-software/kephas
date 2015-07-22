// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntity.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   An entity represents an identifiable type which gets persisted in some form of storage.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model
{
    using Kephas.Model;

    /// <summary>
    /// An entity represents an identifiable type which gets persisted in some form of storage.
    /// </summary>
    public interface IEntity : IReferenceType
    {
    }
}