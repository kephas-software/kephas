// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the find result class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;

    /// <summary>
    /// Encapsulates the result of a find command.
    /// </summary>
    public class FindResult : DataCommandResult, IFindResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FindResult"/> class.
        /// </summary>
        /// <param name="entity">The found entity.</param>
        /// <param name="message">Optional. The message.</param>
        /// <param name="exception">Optional. The exception.</param>
        public FindResult(object? entity, string? message = null, Exception? exception = null)
            : base(message, exception)
        {
            this.Value = this.Entity = entity;
        }

        /// <summary>
        /// Gets the found entity or <c>null</c> if no entity could be found.
        /// </summary>
        /// <value>
        /// The found entity.
        /// </value>
        public object? Entity { get; }
    }
}