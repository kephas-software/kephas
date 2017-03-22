// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateEntityResult.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the create entity result class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;

    /// <summary>
    /// Encapsulates the result of a create entity command.
    /// </summary>
    public class CreateEntityResult : DataCommandResult, ICreateEntityResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateEntityResult"/> class.
        /// </summary>
        /// <param name="entity">The created entity.</param>
        /// <param name="message">The message (optional).</param>
        /// <param name="exception">The exception (optional).</param>
        public CreateEntityResult(object entity, string message = null, Exception exception = null)
            : base(message, exception)
        {
            this.Entity = entity;
        }

        /// <summary>
        /// Gets or sets the created entity.
        /// </summary>
        /// <value>
        /// The created entity or <c>null</c> if no entity could be created.
        /// </value>
        public object Entity { get; set; }
    }
}