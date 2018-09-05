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
        /// <param name="entityInfo">Information describing the entity.</param>
        /// <param name="message">The message (optional).</param>
        /// <param name="exception">The exception (optional).</param>
        public CreateEntityResult(object entity, IEntityInfo entityInfo, string message = null, Exception exception = null)
            : base(message, exception)
        {
            this.Entity = entity;
            this.EntityInfo = entityInfo;
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
        public IEntityInfo EntityInfo { get; }
    }
}