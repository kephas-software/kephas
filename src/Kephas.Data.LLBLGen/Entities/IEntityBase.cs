// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the LLBLGen entity class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen.Entities
{
    using Kephas.Dynamic;

    using SD.LLBLGen.Pro.ORMSupportClasses;

    /// <summary>
    /// Base interface for LLBLGen entities.
    /// </summary>
    public interface IEntityBase : IEntity2, IDynamic
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        long Id { get; set; }
    }
}