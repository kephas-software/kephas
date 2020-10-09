// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateEntityResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the create entity result class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;

    using Kephas.Data.Capabilities;

    /// <summary>
    /// Encapsulates the result of a create entity command.
    /// </summary>
    public class CreateEntityResult : DataCommandResult, ICreateEntityResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateEntityResult"/> class.
        /// </summary>
        /// <param name="entity">The created entity.</param>
        /// <param name="entityEntry">Information describing the entity.</param>
        /// <param name="message">Optional. The message.</param>
        /// <param name="exception">Optional. The exception.</param>
        public CreateEntityResult(object entity, IEntityEntry entityEntry, string? message = null, Exception? exception = null)
            : base(message, exception)
        {
            this.Value = this.Entity = entity;
            this.EntityEntry = entityEntry;
        }

        /// <summary>
        /// Gets the created entity.
        /// </summary>
        /// <value>
        /// The created entity or <c>null</c> if no entity could be created.
        /// </value>
        public object Entity { get; }

        /// <summary>
        /// Gets information describing the entity.
        /// </summary>
        /// <value>
        /// Information describing the entity.
        /// </value>
        public IEntityEntry EntityEntry { get; }
    }
}