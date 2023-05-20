// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICorrelated.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    /// <summary>
    /// Mix-in for correlated entities.
    /// </summary>
    public interface ICorrelated
    {
        /// <summary>
        /// Gets the correlation ID for this instance.
        /// </summary>
        string CorrelationId { get; }
    }
}