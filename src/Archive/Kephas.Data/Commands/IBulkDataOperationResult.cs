// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBulkDataOperationResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IBulkDataOperationResult interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using Kephas.Operations;

    /// <summary>
    /// Interface for returning a bulk operation result.
    /// </summary>
    public interface IBulkDataOperationResult : IOperationResult
    {
        /// <summary>
        /// Gets the number of affected entities.
        /// </summary>
        /// <value>
        /// The number of affected entities.
        /// </value>
        long Count { get; }
    }
}