// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IKey.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IEntityKey interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model
{
    using Kephas.Model;


    /// <summary>
    /// Enumerates the key kinds.
    /// </summary>
    public enum KeyKind
    {
        /// <summary>
        /// Indicates a simple unique key.
        /// </summary>
        Default,

        /// <summary>
        /// Indicates the primary key.
        /// </summary>
        Primary,

        /// <summary>
        /// Indicates a natural key which can be used by humans to identify the entity.
        /// </summary>
        Natural,
    }

    /// <summary>
    /// Interface for entity key.
    /// </summary>
    public interface IKey : IModelElement
    {
        /// <summary>
        /// Gets the key kind.
        /// </summary>
        /// <value>
        /// The key kind.
        /// </value>
        KeyKind Kind { get; }

        /// <summary>
        /// Gets the entity properties in the proper order which are part of the key.
        /// </summary>
        /// <value>
        /// The key properties.
        /// </value>
        IProperty[] KeyProperties { get; }
    }
}