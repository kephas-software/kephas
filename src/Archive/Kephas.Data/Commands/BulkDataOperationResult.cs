// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BulkDataOperationResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the bulk operation result class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;

    /// <summary>
    /// Encapsulates the result of a bulk operation.
    /// </summary>
    public record BulkDataOperationResult : DataCommandResult, IBulkDataOperationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulkDataOperationResult"/> class.
        /// </summary>
        /// <param name="count">The number of affected entities.</param>
        /// <param name="message">Optional. The message.</param>
        /// <param name="exception">Optional. The exception.</param>
        public BulkDataOperationResult(long count, string? message = null, Exception? exception = null)
            : base(message, exception)
        {
            this.Value = this.Count = count;
        }

        /// <summary>
        /// Gets the number of affected entities.
        /// </summary>
        /// <value>
        /// The number of affected entities.
        /// </value>
        public long Count { get; }
    }
}