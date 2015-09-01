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
    using Kephas.Model.Runtime.AttributedModel;

    /// <summary>
    /// Mix-in for identifiable entities.
    /// </summary>
    [Mixin]
    public interface IIdentifiable
    {
        /// <summary>
        /// Gets or sets the identifier for this instance.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        Id Id { get; set; } 
    }
}