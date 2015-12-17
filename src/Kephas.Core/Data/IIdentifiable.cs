// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIdentifiable.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Mix-in for identifiable entities.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    /// <summary>
    /// Mix-in for identifiable entities.
    /// </summary>
    public interface IIdentifiable
    {
        /// <summary>
        /// Gets the identifier for this instance.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        Id Id { get; } 
    }
}