// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIdentifiable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
        object Id { get; }
    }
}